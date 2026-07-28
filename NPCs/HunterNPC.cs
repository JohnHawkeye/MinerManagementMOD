using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

using MinerManagementMOD.Projectiles;

namespace MinerManagementMOD.NPCs
{
    [AutoloadHead]
    public class HunterNPC : ModNPC
    {
        // ==========================================
        // 追従設定
        // ==========================================

        // プレイヤーの後ろに立つ距離
        private const float FollowDistance = 80f;

        // この距離以上離れたらワープ
        private const float TeleportDistance = 500f;

        // 通常追従速度
        private const float FollowSpeed = 3f;

        // 離れた時の追いつき速度
        private const float CatchUpSpeed = 6f;


        // ==========================================
        // 探索・射撃設定
        // ==========================================

        // プレイヤーを中心とした敵の探索範囲
        private const float SearchRange = 600f;

        // ライフルの威力
        private const int RifleDamage = 100;

        // 弾速
        private const float RifleSpeed = 18f;

        // 射撃間隔
        // 60 = 約1秒
        // 180 = 約3秒
        private const int RifleFireInterval = 120;


        // ==========================================
        // Hunter内部データ
        // ==========================================

        private Item weapon;

        private NPC targetEnemy;

        private int shootCooldown = 0;


        private enum HunterState
        {
            Following,
            Aiming
        }

        private HunterState CurrentState =
            HunterState.Following;


        // ==========================================
        // 基本設定
        // ==========================================

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 26;

            NPC.aiStyle = -1;
        }

        public override void SetDefaults()
        {
            NPC.width = 18;
            NPC.height = 40;

            NPC.damage = 0;
            NPC.defense = 15;
            NPC.lifeMax = 300;

            NPC.knockBackResist = 0.3f;

            NPC.friendly = true;
            NPC.townNPC = false;

            NPC.noGravity = false;
            NPC.noTileCollide = false;

            NPC.aiStyle = -1;
        }


        // ==========================================
        // 出現時
        // ==========================================

        public override void OnSpawn(IEntitySource source)
        {
            weapon = new Item();
            weapon.SetDefaults(ItemID.Musket);

            CurrentState = HunterState.Following;

            targetEnemy = null;

            shootCooldown = 0;
        }


        // ==========================================
        // AI
        // ==========================================

        public override void AI()
        {
            // デスポーン防止
            NPC.timeLeft = 60;

            NPC.TargetClosest();

            Player player = Main.player[NPC.target];

            if (!player.active || player.dead)
                return;


            // ------------------------------------------
            // 射撃クールダウン
            // ------------------------------------------

            if (shootCooldown > 0)
                shootCooldown--;


            // ------------------------------------------
            // 状態処理
            // ------------------------------------------

            switch (CurrentState)
            {
                case HunterState.Following:

                    FollowPlayer(player);

                    targetEnemy = FindEnemy(player);

                    if (targetEnemy != null)
                    {
                        CurrentState =
                            HunterState.Aiming;
                    }

                    break;


                case HunterState.Aiming:

                    AimAtEnemy(player);

                    break;
            }
        }


        // ==========================================
        // プレイヤー追従
        // ==========================================

        private void FollowPlayer(Player player)
        {
            float distance =
                Vector2.Distance(
                    NPC.Center,
                    player.Center
                );


            // ------------------------------------------
            // 遠く離れすぎたらワープ
            // ------------------------------------------

            if (distance > TeleportDistance)
            {
                TeleportToPlayer(player);
                return;
            }


            // ------------------------------------------
            // プレイヤーの後方位置
            // ------------------------------------------

            float behindDirection =
                player.direction == 1
                ? -1f
                : 1f;


            Vector2 targetPosition =
                player.Center +
                new Vector2(
                    behindDirection * FollowDistance,
                    0f
                );


            float targetDistance =
                Vector2.Distance(
                    NPC.Center,
                    targetPosition
                );


            // ------------------------------------------
            // 目標地点に近い場合
            // ------------------------------------------

            if (targetDistance < 20f)
            {
                NPC.velocity.X *= 0.85f;
            }
            else
            {
                float speed =
                    distance > 180f
                    ? CatchUpSpeed
                    : FollowSpeed;


                float direction =
                    targetPosition.X > NPC.Center.X
                    ? 1f
                    : -1f;


                NPC.velocity.X +=
                    (direction * speed -
                    NPC.velocity.X) * 0.15f;


                NPC.direction =
                    direction > 0
                    ? 1
                    : -1;

                NPC.spriteDirection =
                    NPC.direction;
            }


            // ------------------------------------------
            // 段差をジャンプ
            // ------------------------------------------

            if (targetPosition.Y + 24 < NPC.Center.Y &&
                NPC.collideY)
            {
                NPC.velocity.Y = -7f;
            }
        }


        // ==========================================
        // プレイヤーのところへワープ
        // ==========================================

        private void TeleportToPlayer(Player player)
        {
            NPC.Center = player.Center;
            NPC.velocity = Vector2.Zero;
            NPC.direction = player.direction;
            NPC.spriteDirection = player.direction;

            SoundEngine.PlaySound(SoundID.Item8, NPC.Center);
        }


        // ==========================================
        // 敵を探す
        // ==========================================

        private NPC FindEnemy(Player player)
        {
            NPC nearest = null;

            float nearestDistance = SearchRange;

            foreach (NPC npc in Main.npc)
            {
                if (!npc.active)
                    continue;

                if (npc.friendly)
                    continue;

                if (npc.life <= 0)
                    continue;

                if (npc.dontTakeDamage)
                    continue;


                // プレイヤーから敵までの距離
                float distance =
                    Vector2.Distance(
                        player.Center,
                        npc.Center
                    );

                // 射程外
                if (distance > SearchRange)
                    continue;


                // Hunterから敵までの射線を確認
                Vector2 start = NPC.Center;
                Vector2 end = npc.Center;

                if (!Collision.CanHitLine(
                        start,
                        1,
                        1,
                        end,
                        1,
                        1))
                {
                    // 壁などに遮られているので、
                    // この敵はターゲット候補から除外
                    continue;
                }


                // 一番近い敵を採用
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearest = npc;
                }
            }

            return nearest;
        }

        // ==========================================
        // 敵を狙う
        // ==========================================

        private void AimAtEnemy(Player player)
        {
            // ------------------------------------------
            // Hunterがプレイヤーから離れすぎた
            // ------------------------------------------

            if (Vector2.Distance(
                    NPC.Center,
                    player.Center) > TeleportDistance)
            {
                targetEnemy = null;

                CurrentState =
                    HunterState.Following;

                return;
            }


            // ------------------------------------------
            // ターゲットが死亡・消滅
            // ------------------------------------------

            if (targetEnemy == null ||
                !targetEnemy.active ||
                targetEnemy.life <= 0)
            {
                targetEnemy = null;

                CurrentState =
                    HunterState.Following;

                return;
            }


            // ------------------------------------------
            // ターゲットが射程外
            // ------------------------------------------

            float enemyDistance =
                Vector2.Distance(
                    player.Center,
                    targetEnemy.Center
                );


            if (enemyDistance > SearchRange)
            {
                targetEnemy = null;

                CurrentState =
                    HunterState.Following;

                return;
            }


            // ------------------------------------------
            // Hunterは敵を追いかけない
            // ------------------------------------------

            NPC.velocity.X *= 0.85f;


            // ------------------------------------------
            // 敵の方向を向く
            // ------------------------------------------

            NPC.direction =
                targetEnemy.Center.X >
                NPC.Center.X
                ? 1
                : -1;

            NPC.spriteDirection =
                NPC.direction;


            // ------------------------------------------
            // 射撃
            // ------------------------------------------

            if (shootCooldown <= 0)
            {
                ShootEnemy();

                shootCooldown =
                    RifleFireInterval;
            }
        }


        // ==========================================
        // ライフル射撃
        // ==========================================

        private void ShootEnemy()
        {
            if (targetEnemy == null)
                return;

            if (!targetEnemy.active)
                return;

            if (targetEnemy.life <= 0)
                return;


            // ------------------------------------------
            // Hunter → 敵への方向
            // ------------------------------------------

            Vector2 direction =
                targetEnemy.Center -
                NPC.Center;


            if (direction.LengthSquared() <= 0f)
                return;


            direction.Normalize();


            Vector2 velocity =
                direction * RifleSpeed;


            // ------------------------------------------
            // Projectile生成
            // ------------------------------------------

            Projectile.NewProjectile(
                NPC.GetSource_FromAI(),
                NPC.Center,
                velocity,
                ModContent.ProjectileType<HunterRifleProjectile>(),
                RifleDamage,
                4f,
                Main.myPlayer
            );


            // ------------------------------------------
            // ライフル発射音
            // ------------------------------------------

            SoundEngine.PlaySound(
                SoundID.Item11,
                NPC.Center
            );
        }


        // ==========================================
        // ライフル描画
        // ==========================================

        private void DrawRifle(
            SpriteBatch spriteBatch,
            Vector2 screenPos)
        {
            if (weapon == null)
                return;


            Texture2D texture =
                TextureAssets.Item[
                    weapon.type
                ].Value;


            SpriteEffects effects =
                NPC.spriteDirection == -1
                ? SpriteEffects.FlipHorizontally
                : SpriteEffects.None;


            Vector2 handOffset =
                new Vector2(
                    8f * NPC.direction,
                    -8f
                );


            Vector2 drawPos =
                NPC.Center +
                handOffset -
                screenPos;


            Vector2 origin =
                new Vector2(
                    texture.Width * 0.15f,
                    texture.Height * 0.5f
                );


            Vector2 targetDirection;


            if (targetEnemy != null &&
                targetEnemy.active &&
                targetEnemy.life > 0)
            {
                targetDirection =
                    targetEnemy.Center -
                    NPC.Center;
            }
            else
            {
                targetDirection =
                    new Vector2(
                        NPC.direction,
                        0f
                    );
            }


            float rotation =
                targetDirection.ToRotation();


            spriteBatch.Draw(
                texture,
                drawPos,
                null,
                Lighting.GetColor(
                    (int)NPC.Center.X / 16,
                    (int)NPC.Center.Y / 16
                ),
                rotation,
                origin,
                1f,
                effects,
                0f
            );
        }
        private void DrawObsidianArmor(
            SpriteBatch spriteBatch,
            Vector2 screenPos)
        {

            Main.instance.LoadArmorHead(
                ArmorIDs.Head.ObsidianOutlawHat);

            Main.instance.LoadArmorBody(
                ArmorIDs.Body.ObsidianLongcoat);

            Main.instance.LoadArmorLegs(
                ArmorIDs.Legs.ObsidianPants);

            Texture2D helmet =
                TextureAssets.ArmorHead[
                    ArmorIDs.Head.ObsidianOutlawHat
                ].Value;

            Texture2D body =
                TextureAssets.ArmorBody[
                    ArmorIDs.Body.ObsidianLongcoat
                ].Value;

            Texture2D legs =
                TextureAssets.ArmorLeg[
                    ArmorIDs.Legs.ObsidianPants
                ].Value;

            SpriteEffects effects =
                NPC.spriteDirection == -1
                    ? SpriteEffects.FlipHorizontally
                    : SpriteEffects.None;

            Color color =
                Lighting.GetColor(
                    (int)NPC.Center.X / 16,
                    (int)NPC.Center.Y / 16);

            // 胴
            spriteBatch.Draw(
                body,
                NPC.Center - screenPos + new Vector2(0f, 2f),
                null,
                color,
                0f,
                new Vector2(
                    body.Width / 2f,
                    body.Height / 2f),
                1f,
                effects,
                0f);

            // 脚
            spriteBatch.Draw(
                legs,
                NPC.Center - screenPos + new Vector2(0f, 14f),
                null,
                color,
                0f,
                new Vector2(
                    legs.Width / 2f,
                    legs.Height / 2f),
                1f,
                effects,
                0f);

            // 頭
            spriteBatch.Draw(
                helmet,
                NPC.Center - screenPos + new Vector2(0f, -15f),
                null,
                color,
                0f,
                new Vector2(
                    helmet.Width / 2f,
                    helmet.Height / 2f),
                1f,
                effects,
                0f);
        }

        // ==========================================
        // 描画
        // ==========================================

        public override void PostDraw(
            SpriteBatch spriteBatch,
            Vector2 screenPos,
            Color drawColor)
        {
            DrawObsidianArmor(spriteBatch, screenPos);
            DrawRifle(
                spriteBatch,
                screenPos
            );
        }


        // ==========================================
        // フレーム
        // ==========================================

        public override void FindFrame(
            int frameHeight)
        {
            // 空中
            if (!NPC.collideY)
            {
                NPC.frame.Y =
                    frameHeight * 15;

                return;
            }


            // 歩行
            if (System.Math.Abs(
                    NPC.velocity.X) > 0.1f)
            {
                NPC.frameCounter++;


                if (NPC.frameCounter >= 6)
                {
                    NPC.frameCounter = 0;

                    NPC.frame.Y += frameHeight;


                    if (NPC.frame.Y <
                            frameHeight * 7 ||
                        NPC.frame.Y >
                            frameHeight * 15)
                    {
                        NPC.frame.Y =
                            frameHeight * 7;
                    }
                }
            }
            else
            {
                // 待機
                NPC.frameCounter++;


                if (NPC.frameCounter >= 12)
                {
                    NPC.frameCounter = 0;

                    NPC.frame.Y += frameHeight;


                    if (NPC.frame.Y >
                        frameHeight * 7)
                    {
                        NPC.frame.Y = 0;
                    }
                }
            }
        }


        // ==========================================
        // 会話
        // ==========================================

        public override bool CanChat()
        {
            return true;
        }


        public override string GetChat()
        {
            return "周囲の警戒は任せてください。";
        }
    }
}