using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using MinerManagementMOD.Helpers;
using Terraria.Audio;
using System.Collections.Generic;
using MinerManagementMOD.Projectiles;

namespace MinerManagementMOD.NPCs
{
    [AutoloadHead]
    public class MagicMinerNPC : ModNPC
    {
        //data paras
        public int MinerID;
        public string MinerName;
        public int MiningLevel;
        public int MiningPower;
        public int MiningSpeed;
        public int CarryCapacity;
        public int OreBonusChance;
        public bool HasLight;

        private const float TeleportDistance = 500f;

        private const int DetectRange = 12;
        private Point targetTile = Point.Zero;
        private bool hasTarget = false;

        private int spellCooldown = 0;
        private const int SpellCooldownMax = 120;

        // 魔法採掘師が探知する鉱物
        private static readonly HashSet<int> NormalOres = new()
        {
            TileID.Copper,
            TileID.Tin,
            TileID.Iron,
            TileID.Lead,
            TileID.Silver,
            TileID.Tungsten,
            TileID.Gold,
            TileID.Platinum,
            TileID.Hellstone,
            TileID.Meteorite,

            // その他鉱石
            TileID.Demonite,
            TileID.Crimtane,
            TileID.Hellstone,
            TileID.Meteorite,
            
                        // 宝石
            TileID.Amethyst,
            TileID.Topaz,
            TileID.Sapphire,
            TileID.Emerald,
            TileID.Ruby,
            TileID.Diamond,
        };

        private static readonly HashSet<int> HardmodeOres = new()
        {
            TileID.Cobalt,
            TileID.Palladium,
            TileID.Mythril,
            TileID.Orichalcum,
            TileID.Adamantite,
            TileID.Titanium,
            TileID.Chlorophyte
        };

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = Main.npcFrameCount[NPCID.Wizard];
        }

        public override void SetDefaults()
        {
            NPC.width = 32;
            NPC.height = 48;

            NPC.damage = 10;
            NPC.defense = 5;
            NPC.lifeMax = 100;

            NPC.knockBackResist = 0.5f;

            NPC.friendly = true;

            // 独自AI
            NPC.aiStyle = -1;

            NPC.noGravity = false;
            NPC.noTileCollide = false;
        }

        public override bool CanChat()
        {
            return true;
        }

        public override void SetChatButtons(
            ref string button,
            ref string button2)
        {
            // 魔法採掘師は「休ませる」だけ
            button = "休んでもらう";
            button2 = "";
        }

        public override void OnChatButtonClicked(
            bool firstButton,
            ref string shopName)
        {
            if (firstButton)
            {
                Rest();
            }
        }

        public override string GetChat()
        {
            return "魔力を使って鉱石を探知し、採掘します。";
        }

        public void Rest()
        {
            Main.NewText($"{NPC.GivenName} : またよろしくお願いします。", 255, 255, 0);

            NPC.active = false;
        }

        public override void AI()
        {
            NPC.timeLeft = 60;
            if (HasLight)
            {
                Lighting.AddLight(NPC.Center, 1f, 0.95f, 0.75f);
            }

            // 常にプレイヤーをターゲット
            NPC.target = Player.FindClosest(NPC.position, NPC.width, NPC.height);

            Player player = Main.player[NPC.target];

            // プレイヤーが無効
            if (!player.active || player.dead)
            {
                NPC.velocity.X *= 0.9f;
                return;
            }

            if (spellCooldown > 0)
            {
                spellCooldown--;
            }

            // プレイヤーの一歩後ろを追従
            FollowPlayer(player);

            // 周囲の採掘可能ブロックを探知
            FindNearbyMineableTile();

            // ターゲットが見つかった場合
            if (hasTarget)
            {
                AimAtTarget();

                if (spellCooldown <= 0)
                {
                    FireMiningMagic();
                }
            }
        }

        // =========================================================
        // プレイヤー追従
        // =========================================================

        private void FollowPlayer(Player player)
        {
            // プレイヤーの一歩後ろ
            Vector2 targetPosition =
                player.Center -
                new Vector2(player.direction * 32f, 0f);

            Vector2 difference = targetPosition - NPC.Center;

            float distance = difference.Length();

            // ------------------------------------------
            // 遠く離れすぎたらワープ
            // ------------------------------------------

            if (distance > TeleportDistance)
            {
                TeleportToPlayer(player);
                return;
            }

            // 十分近ければ停止
            if (distance < 4f)
            {
                NPC.velocity.X *= 0.8f;
                return;
            }

            float speed = 4f;

            Vector2 move =
                difference.SafeNormalize(Vector2.Zero) * speed;

            NPC.velocity.X = move.X;

            // 向きを変更
            if (NPC.velocity.X != 0f)
            {
                NPC.direction =
                    NPC.velocity.X > 0f ? 1 : -1;

                NPC.spriteDirection = NPC.direction;
            }

            //jump
            if (player.Center.Y + 24 < NPC.Center.Y &&
                NPC.collideY)
            {
                NPC.velocity.Y = -7f;
            }
        }

        private void TeleportToPlayer(Player player)
        {
            NPC.Center = player.Center;
            NPC.velocity = Vector2.Zero;
            NPC.direction = player.direction;
            NPC.spriteDirection = player.direction;

            SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
        }

        // =========================================================
        // 採掘可能ブロック探知
        // =========================================================

        private void FindNearbyMineableTile()
        {
            hasTarget = false;

            int centerX = (int)(NPC.Center.X / 16f);
            int centerY = (int)(NPC.Center.Y / 16f);

            float closestDistance = float.MaxValue;


            for (int x = centerX - DetectRange;
                 x <= centerX + DetectRange;
                 x++)
            {
                for (int y = centerY - DetectRange;
                     y <= centerY + DetectRange;
                     y++)
                {
                    // ワールド外
                    if (!WorldGen.InWorld(x, y))
                        continue;

                    // 円形の20ブロック範囲
                    float distance =
                        Vector2.Distance(
                            new Vector2(centerX, centerY),
                            new Vector2(x, y));

                    if (distance > DetectRange)
                        continue;

                    Tile tile = Framing.GetTileSafely(x, y);

                    // タイルが存在しない
                    if (!tile.HasTile)
                        continue;

                    // ------------------------------------------
                    // 鉱物判定
                    // ------------------------------------------

                    bool isOre = NormalOres.Contains(tile.TileType);

                    // MiningLevel 2ならハードモード鉱石も対象
                    if (MiningLevel >= 2)
                    {
                        isOre |= HardmodeOres.Contains(tile.TileType);
                    }

                    // 鉱物ではない
                    if (!isOre)
                        continue;

                    // 現在の最短距離より遠いなら無視
                    if (distance >= closestDistance)
                        continue;

                    closestDistance = distance;

                    targetTile = new Point(x, y);
                    hasTarget = true;
                }
            }
        }

        private void FireMiningMagic()
        {
            Vector2 targetPosition =
                new Vector2(
                    targetTile.X * 16f + 8f,
                    targetTile.Y * 16f + 8f);

            Vector2 direction =
                targetPosition - NPC.Center;

            if (direction.LengthSquared() <= 0f)
                return;

            direction.Normalize();

            float speed = 10f;

            int projectileType =
                ModContent.ProjectileType<MagicMiningProjectile>();

            int projectileIndex = Projectile.NewProjectile(
                NPC.GetSource_FromAI(),
                NPC.Center,
                direction * speed,
                projectileType,
                0,
                0f,
                Main.myPlayer,
                targetPosition.X,
                targetPosition.Y
            );

            if (projectileIndex >= 0 &&
                    projectileIndex < Main.maxProjectiles)
            {
                if (Main.projectile[projectileIndex].ModProjectile
                    is MagicMiningProjectile magicProjectile)
                {
                    magicProjectile.MiningLevel = MiningLevel;
                }
            }
            // クールタイム開始
            spellCooldown = SpellCooldownMax;
        }

        // =========================================================
        // ターゲットの方向を向く
        // =========================================================

        private void AimAtTarget()
        {
            Vector2 targetPosition =
                new Vector2(
                    targetTile.X * 16f + 8f,
                    targetTile.Y * 16f + 8f);

            Vector2 direction =
                targetPosition - NPC.Center;

            if (direction.LengthSquared() <= 0f)
                return;

            direction.Normalize();

            // ターゲット方向を向く
            NPC.direction =
                direction.X >= 0f ? 1 : -1;

            NPC.spriteDirection = NPC.direction;

            // =====================================================
            // 後でProjectile発射に使用する
            // =====================================================

            NPC.localAI[0] = direction.X;
            NPC.localAI[1] = direction.Y;
        }

        // =========================================================
        // アニメーション
        // =========================================================

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;

            if (NPC.frameCounter >= 10)
            {
                NPC.frameCounter = 0;

                NPC.frame.Y += frameHeight;

                if (NPC.frame.Y >=
                    frameHeight * Main.npcFrameCount[Type])
                {
                    NPC.frame.Y = 0;
                }
            }
        }
    }
}