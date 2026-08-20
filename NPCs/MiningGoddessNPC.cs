using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using MinerManagementMOD.Items;
using Terraria.GameContent.Personalities;
using Terraria.GameContent.Bestiary;

namespace MinerManagementMOD.NPCs
{
    [AutoloadHead]
    public class MiningGoddessNPC : ModNPC
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

        // =========================================================
        // 基本設定
        // =========================================================

        private const int FrameCount = 6;
        private const int FrameWidth = 40;
        private const int FrameHeight = 56;

        // プレイヤーからの追従位置
        private const float FollowDistance = 48f;
        private const float FollowHeight = 28f;

        // 鉱石探索範囲
        private const int MiningRadius = 30;

        // 貢ぐアニメーション
        private const int OfferingTime = 60; // 1秒 = 60フレーム

        // 採掘後、アイテム回収を開始するまで
        private const int CollectDelay = 40;

        // アイテム吸引速度
        private const float CollectSpeed = 24f;

        // =========================================================
        // 状態
        // =========================================================

        private enum GoddessState
        {
            Idle,
            Offering,
            WaitingForCollect,
            Collecting
        }

        private GoddessState State = GoddessState.Idle;

        // 現在の状態で何フレーム経過したか
        private int StateTimer = 0;

        // 採掘したときに発生したアイテム
        private readonly List<int> CollectedItems = new();

        // 誰に追従するか
        private int TargetPlayer = -1;


        // =========================================================
        // Static Defaults
        // =========================================================

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = FrameCount;

            NPCID.Sets.NPCBestiaryDrawModifiers value =
                new NPCID.Sets.NPCBestiaryDrawModifiers
                {
                    Velocity = 1f
                };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }


        // =========================================================
        // Defaults
        // =========================================================

        public override void SetDefaults()
        {
            NPC.width = FrameWidth;
            NPC.height = FrameHeight;

            NPC.damage = 0;
            NPC.defense = 9999;

            NPC.lifeMax = 999999;
            NPC.knockBackResist = 0f;

            // 無敵
            NPC.dontTakeDamage = true;

            // タイルをすり抜ける
            NPC.noGravity = true;
            NPC.noTileCollide = true;

            // 敵に狙われにくくする
            NPC.friendly = true;
            NPC.chaseable = false;

            // Town NPCとして扱う
            //NPC.townNPC = true;

            // AIは自前
            NPC.aiStyle = -1;

            // 落下・ノックバック等を完全に無効化
            NPC.netAlways = true;

            // 会話可能
            NPC.Happiness
                .SetBiomeAffection<ForestBiome>(AffectionLevel.Love);
        }


        // =========================================================
        // AI
        // =========================================================

        public override void AI()
        {
            // -----------------------------------------------------
            // 追従対象プレイヤーを探す
            // -----------------------------------------------------

            Player player = GetTargetPlayer();

            if (player == null || !player.active)
            {
                NPC.velocity = Vector2.Zero;
                return;
            }

            TargetPlayer = player.whoAmI;

            // -----------------------------------------------------
            // 状態ごとの処理
            // -----------------------------------------------------

            switch (State)
            {
                case GoddessState.Idle:
                    UpdateIdle(player);
                    break;

                case GoddessState.Offering:
                    UpdateOffering(player);
                    break;

                case GoddessState.WaitingForCollect:
                    UpdateWaitingForCollect(player);
                    break;

                case GoddessState.Collecting:
                    UpdateCollecting(player);
                    break;
            }

            // -----------------------------------------------------
            // 常に重力・衝突なし
            // -----------------------------------------------------

            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.dontTakeDamage = true;

            NPC.velocity = Vector2.Zero;

            // 光
            if (State == GoddessState.Offering)
            {
                Lighting.AddLight(
                    NPC.Center,
                    1.0f,
                    0.8f,
                    0.35f
                );
            }
        }


        // =========================================================
        // プレイヤー取得
        // =========================================================

        private Player GetTargetPlayer()
        {
            if (TargetPlayer >= 0 &&
                TargetPlayer < Main.maxPlayers &&
                Main.player[TargetPlayer].active)
            {
                return Main.player[TargetPlayer];
            }

            // 最も近いプレイヤーを探す
            Player closest = null;
            float closestDistance = float.MaxValue;

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];

                if (!player.active)
                    continue;

                float distance =
                    Vector2.DistanceSquared(
                        NPC.Center,
                        player.Center
                    );

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closest = player;
                }
            }

            return closest;
        }


        // =========================================================
        // 通常待機
        // =========================================================

        private void UpdateIdle(Player player)
        {
            StateTimer++;

            MoveBehindPlayer(player);

            // 待機中は1～3フレーム
            AnimateIdle();
        }


        // =========================================================
        // プレイヤーの背後へ追従
        // =========================================================

        private void MoveBehindPlayer(Player player)
        {
            Vector2 targetPosition =
                player.Center +
                new Vector2(
                    -player.direction * FollowDistance,
                    -FollowHeight
                );

            // ゆっくり追従
            NPC.Center = Vector2.Lerp(
                NPC.Center,
                targetPosition,
                0.18f
            );

            NPC.velocity = Vector2.Zero;

            // プレイヤーと同じ方向を向く
            NPC.direction = player.direction;
            NPC.spriteDirection = player.direction;
        }


        // =========================================================
        // 待機アニメーション 1,2,3
        // =========================================================

        private void AnimateIdle()
        {
            // 12フレームごとに切り替え
            int frame = (StateTimer / 12) % 3;

            NPC.frame.Y = frame * FrameHeight;
        }


        // =========================================================
        // 貢ぐ開始
        // =========================================================

        private void StartOffering(Player player)
        {
            if (State != GoddessState.Idle)
                return;

            // ゴールドマイナーコイン1枚
            int coinType =
                ModContent.ItemType<GoldMinerCoin>();

            if (!player.HasItem(coinType))
            {
                Main.npcChatText =
                    "ゴールドマイナーコインが必要です。";

                return;
            }

            // コイン消費
            player.ConsumeItem(coinType);

            TargetPlayer = player.whoAmI;

            State = GoddessState.Offering;
            StateTimer = 0;

            CollectedItems.Clear();

            NPC.netUpdate = true;

            // 開始音
            SoundEngine.PlaySound(
                SoundID.Item29,
                NPC.Center
            );

            // 開始時の光
            CreateHolyEffect();
        }


        // =========================================================
        // 奉納アニメーション
        // =========================================================

        private void UpdateOffering(Player player)
        {
            StateTimer++;

            MoveBehindPlayer(player);

            // -----------------------------------------------------
            // 4 → 5 → 6
            // -----------------------------------------------------

            int frame;

            if (StateTimer < 20)
            {
                // 4
                frame = 3;
            }
            else if (StateTimer < 40)
            {
                // 5
                frame = 4;
            }
            else
            {
                // 6
                frame = 5;
            }

            NPC.frame.Y = frame * FrameHeight;

            // -----------------------------------------------------
            // 神聖な光
            // -----------------------------------------------------

            CreateHolyEffect();

            // -----------------------------------------------------
            // 60フレームで採掘
            // -----------------------------------------------------

            if (StateTimer >= OfferingTime)
            {
                PerformGoddessMining(player);

                State = GoddessState.WaitingForCollect;
                StateTimer = 0;

                NPC.netUpdate = true;
            }
        }


        // =========================================================
        // 神聖な光エフェクト
        // =========================================================

        private void CreateHolyEffect()
        {
            if (Main.rand.NextBool(2))
            {
                Vector2 offset =
                    Main.rand.NextVector2Circular(
                        35f,
                        35f
                    );

                Dust dust = Dust.NewDustPerfect(
                    NPC.Center + offset,
                    DustID.GoldFlame,
                    Vector2.Zero,
                    100,
                    default,
                    Main.rand.NextFloat(1.0f, 1.6f)
                );

                dust.noGravity = true;
            }

            Lighting.AddLight(
                NPC.Center,
                1.2f,
                1.0f,
                0.45f
            );
        }


        // =========================================================
        // 女神による採掘
        // =========================================================

        private void PerformGoddessMining(Player player)
        {
            // マルチプレイではサーバーだけが実行
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            int centerX =
                (int)(player.Center.X / 16f);

            int centerY =
                (int)(player.Center.Y / 16f);

            int radius = MiningRadius;

            int minX = centerX - radius;
            int maxX = centerX + radius;

            int minY = centerY - radius;
            int maxY = centerY + radius;

            minX = Utils.Clamp(
                minX,
                0,
                Main.maxTilesX - 1
            );

            maxX = Utils.Clamp(
                maxX,
                0,
                Main.maxTilesX - 1
            );

            minY = Utils.Clamp(
                minY,
                0,
                Main.maxTilesY - 1
            );

            maxY = Utils.Clamp(
                maxY,
                0,
                Main.maxTilesY - 1
            );


            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    float dx = x - centerX;
                    float dy = y - centerY;

                    // 円形の30ブロック範囲
                    if (dx * dx + dy * dy >
                        radius * radius)
                    {
                        continue;
                    }

                    Tile tile =
                        Framing.GetTileSafely(x, y);

                    if (!tile.HasTile)
                        continue;

                    int tileType = tile.TileType;

                    // 鉱石だけ
                    if (!IsTargetOre(tileType))
                        continue;

                    // 破壊可能か確認
                    if (!WorldGen.CanKillTile(x, y))
                        continue;

                    // タイル破壊
                    WorldGen.KillTile(
                        x,
                        y,
                        false,
                        false,
                        false
                    );

                    // ネットワーク同期
                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendTileSquare(
                            -1,
                            x,
                            y,
                            1
                        );
                    }
                }
            }

            // 大きな採掘エフェクト
            CreateMiningExplosion(player);

            SoundEngine.PlaySound(
                SoundID.Item70,
                player.Center
            );
        }


        // =========================================================
        // 鉱石判定
        // =========================================================

        private bool IsTargetOre(int type)
        {
            switch (type)
            {
                // -------------------------------------------------
                // Pre-Hardmode
                // -------------------------------------------------

                case TileID.Copper:
                case TileID.Tin:

                case TileID.Iron:
                case TileID.Lead:

                case TileID.Silver:
                case TileID.Tungsten:

                case TileID.Gold:
                case TileID.Platinum:

                // -------------------------------------------------
                // その他の鉱石
                // -------------------------------------------------

                case TileID.Meteorite:

                case TileID.Demonite:
                case TileID.Crimtane:

                // -------------------------------------------------
                // Hardmode
                // -------------------------------------------------

                case TileID.Cobalt:
                case TileID.Palladium:

                case TileID.Mythril:
                case TileID.Orichalcum:

                case TileID.Adamantite:
                case TileID.Titanium:

                // -------------------------------------------------
                // Chlorophyte
                // -------------------------------------------------

                case TileID.Chlorophyte:

                //juwel
                case TileID.ExposedGems:
                case TileID.Amethyst:
                case TileID.Topaz:
                case TileID.Sapphire:
                case TileID.Emerald:
                case TileID.Ruby:
                case TileID.Diamond:
                case TileID.Hellstone:
                
                return true;
            }

            return false;
        }


        // =========================================================
        // 採掘エフェクト
        // =========================================================

        private void CreateMiningExplosion(Player player)
        {
            for (int i = 0; i < 80; i++)
            {
                Vector2 velocity =
                    Main.rand.NextVector2CircularEdge(
                        5f,
                        5f
                    );

                Dust dust = Dust.NewDustPerfect(
                    player.Center,
                    DustID.GoldFlame,
                    velocity,
                    100,
                    default,
                    Main.rand.NextFloat(
                        1.0f,
                        2.0f
                    )
                );

                dust.noGravity = true;
            }

            Lighting.AddLight(
                player.Center,
                2f,
                1.6f,
                0.6f
            );
        }


        // =========================================================
        // 40フレーム待機
        // =========================================================

        private void UpdateWaitingForCollect(Player player)
        {
            StateTimer++;

            MoveBehindPlayer(player);

            // 通常待機アニメーションへ戻す
            AnimateIdle();

            if (StateTimer >= CollectDelay)
            {
                State =
                    GoddessState.Collecting;

                StateTimer = 0;

                NPC.netUpdate = true;
            }
        }


        // =========================================================
        // アイテム吸収
        // =========================================================

        private void UpdateCollecting(Player player)
        {
            StateTimer++;

            MoveBehindPlayer(player);

            // 通常待機アニメーション
            AnimateIdle();

            // 女神が採掘した範囲
            Vector2 miningCenter = player.Center;

            float collectRadius = MiningRadius * 16f;
            float collectRadiusSquared =
                collectRadius * collectRadius;

            for (int i = 0; i < Main.maxItems; i++)
            {
                Item item = Main.item[i];

                if (!item.active || item.IsAir)
                    continue;

                // 鉱石アイテム以外は無視
                if (!IsOreItem(item.type))
                    continue;

                // 採掘範囲外なら無視
                Vector2 difference =
                    item.Center - miningCenter;

                if (difference.LengthSquared() >
                    collectRadiusSquared)
                {
                    continue;
                }

                // ==========================================
                // 鉱石に神聖な光をまとわせる
                // ==========================================

                Lighting.AddLight(
                    item.Center,
                    0.8f,
                    0.65f,
                    0.25f
                );

                // 金色の光の粒
                if (Main.rand.NextBool(2))
                {
                    Vector2 offset =
                        Main.rand.NextVector2Circular(
                            item.width * 0.5f + 4f,
                            item.height * 0.5f + 4f
                        );

                    Dust dust = Dust.NewDustPerfect(
                        item.Center + offset,
                        DustID.GoldFlame,
                        Vector2.Zero,
                        100,
                        default,
                        Main.rand.NextFloat(0.6f, 1.1f)
                    );

                    dust.noGravity = true;
                }

                // ==========================================
                // 障害物を完全に無視してプレイヤーへ移動
                // ==========================================

                Vector2 toPlayer =
                    player.Center - item.Center;

                float distance = toPlayer.Length();

                // プレイヤーに到達
                if (distance <= 32f)
                {
                    item.position =
                        player.Center - item.Size * 0.5f;

                    item.velocity = Vector2.Zero;

                    // すぐプレイヤーに拾わせる
                    item.noGrabDelay = 0;

                    continue;
                }

                // 正規化
                if (distance > 0.001f)
                {
                    toPlayer /= distance;
                }

                // ==========================================
                // 直線的に吸い寄せる
                // ==========================================

                float speed = 28f;

                // 残り距離が短い場合はオーバーシュートしない
                if (distance < speed)
                {
                    speed = distance;
                }

                item.Center += toPlayer * speed;

                // Terrariaの物理速度を無効化
                item.velocity = Vector2.Zero;

                // タイルとの衝突による引っ掛かりを防止
                item.noGrabDelay = 0;
            }

            // ==========================================
            // 回収終了
            // ==========================================

            if (StateTimer >= 90)
            {
                State = GoddessState.Idle;
                StateTimer = 0;

                NPC.netUpdate = true;
            }
        }


        // =========================================================
        // 鉱石アイテム判定
        // =========================================================

        private bool IsOreItem(int type)
        {
            return
                type == ItemID.CopperOre ||
                type == ItemID.TinOre ||
                type == ItemID.IronOre ||
                type == ItemID.LeadOre ||
                type == ItemID.SilverOre ||
                type == ItemID.TungstenOre ||
                type == ItemID.GoldOre ||
                type == ItemID.PlatinumOre ||
                type == ItemID.Meteorite ||
                type == ItemID.DemoniteOre ||
                type == ItemID.CrimtaneOre ||
                type == ItemID.Hellstone ||
                type == ItemID.CobaltOre ||
                type == ItemID.PalladiumOre ||
                type == ItemID.MythrilOre ||
                type == ItemID.OrichalcumOre ||
                type == ItemID.AdamantiteOre ||
                type == ItemID.TitaniumOre ||
                type == ItemID.ChlorophyteOre ||
                type == ItemID.Amber ||
                type == ItemID.Amethyst ||
                type == ItemID.Topaz ||
                type == ItemID.Sapphire ||
                type == ItemID.Emerald ||
                type == ItemID.Ruby ||
                type == ItemID.Diamond;
        }


        // =========================================================
        // フレーム
        // =========================================================

        public override void FindFrame(int frameHeight)
        {
            // AI側でフレームを管理する
        }


        // =========================================================
        // 会話
        // =========================================================
        public override bool CanChat()
        {
            return true;
        }
        
        public override string GetChat()
        {
            return "鉱山の恵みを求めるのですね……。";
        }


        // =========================================================
        // 会話ボタン
        // =========================================================

        public override void SetChatButtons(
            ref string button,
            ref string button2)
        {
            if (State == GoddessState.Idle)
            {
                button = "貢ぐ";
            }
            else
            {
                button = "";
            }

            button2 = "";
        }


        // =========================================================
        // ボタンが押された
        // =========================================================

        public override void OnChatButtonClicked(
            bool firstButton,
            ref string shopName)
        {
            if (!firstButton)
                return;

            if (State != GoddessState.Idle)
                return;

            Player player = Main.LocalPlayer;

            if (player == null || !player.active)
                return;

            StartOffering(player);
        }


        // =========================================================
        // NPCが死亡しないようにする
        // =========================================================

        public override bool CheckDead()
        {
            NPC.life = NPC.lifeMax;
            NPC.active = true;

            return false;
        }


        // =========================================================
        // ダメージ完全無効
        // =========================================================

        public override bool? CanBeHitByItem(
            Player player,
            Item item)
        {
            return false;
        }

        public override bool? CanBeHitByProjectile(
            Projectile projectile)
        {
            return false;
        }


        // =========================================================
        // Bestiary
        // =========================================================

        public override void SetBestiary(
            BestiaryDatabase database,
            BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.Add(
                new FlavorTextBestiaryInfoElement(
                    "鉱山の恵みを司る女神。ゴールドマイナーコインを捧げることで、周囲の鉱石を一瞬で採掘する。"
                )
            );
        }
    }
}