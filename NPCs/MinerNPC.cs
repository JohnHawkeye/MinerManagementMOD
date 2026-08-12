using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using MinerManagementMOD.Items;
using Terraria.Audio;
using MinerManagementMOD.Systems;

namespace MinerManagementMOD.NPCs
{
    [AutoloadHead]
    public class MinerNPC : ModNPC
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

        //
        public bool IsMining = false;
        public MinerState CurrentState = MinerState.Idle;

        private Vector2 miningStartPosition;
        private const float MaxMiningTravel = 2000f;

        private Item miningTool;

        private bool isSwingingPickaxe = false;
        private int swingFrame = 0;

        private int miningDirection = 1;
        private Point miningColumn;
        private int miningTimer;
        private float advanceTargetX;

        public int MinedBlockCount = 0;
        private const int MaxMineBlocks = 150;
        private int fallingWaitTimer = 0;

        private int torchCounter = 0;
        private const int TorchInterval = 16;

        public enum MinerState
        {
            Idle,
            MovingToMiningArea,
            Mining,
            FallingMining,
            OreMiningMode,
            Advancing,
            KnockedOut
        }

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
            NPC.defense = 10;
            NPC.lifeMax = 250;

            NPC.knockBackResist = 0.4f;

            NPC.friendly = true;
            NPC.townNPC = false;

            NPC.noGravity = false;
            NPC.noTileCollide = false;

            NPC.aiStyle = -1;


        }
        public override void OnSpawn(IEntitySource source)
        {
            miningTool = new Item();
            miningTool.SetDefaults(ItemID.CopperPickaxe);

        }


        public override void AI()
        {
            NPC.timeLeft = 60;
            if (HasLight)
            {
                Lighting.AddLight(NPC.Center, 1f, 0.95f, 0.75f);
            }

            switch (CurrentState)
            {
                case MinerState.Idle:
                    Idle();
                    break;

                case MinerState.MovingToMiningArea:
                    MoveMiningDirection();
                    break;

                case MinerState.Mining:
                    MineNextBlock();
                    break;

                case MinerState.FallingMining:
                    MineFallingBlocks();
                    break;

                case MinerState.OreMiningMode:
                    MineNearbyOre();
                    break;

                case MinerState.Advancing:
                    AdvanceMining();
                    break;
            }
        }

        private void Idle()
        {
            NPC.velocity.X *= 0.8f;

            if (Math.Abs(NPC.velocity.X) < 0.8f)
                NPC.velocity.X = 0f;
        }

        private int GetMiningDelay()
        {
            if (miningTool == null || miningTool.IsAir)
                return 60;

            int useTime = miningTool.useTime;

            // 安全用
            if (useTime <= 0)
                useTime = 60;

            return useTime;
        }

        //sart mining
        public void StartMining()
        {
            Player player = Main.player[Main.myPlayer];

            miningDirection = player.direction;

            NPC.direction = miningDirection;
            NPC.spriteDirection = miningDirection;

            miningStartPosition = NPC.Center;

            IsMining = true;
            MinedBlockCount = 0;

            CurrentState = MinerState.MovingToMiningArea;
        }

        private void MineNextBlock()
        {
            miningTimer++;

            if (miningTimer < GetMiningDelay())
            {
                isSwingingPickaxe = true;
                return;
            }

            miningTimer = 0;
            isSwingingPickaxe = false;

            int targetX = miningColumn.X;

            bool hasFallingBlock = false;

            // 下 → 中 → 上 の順に確認
            int[] checkOrder = { 0, 1, 2 };

            foreach (int offset in checkOrder)
            {
                int targetY = miningColumn.Y - offset;

                Tile tile = Framing.GetTileSafely(targetX, targetY);

                if (!tile.HasTile)
                    continue;

                // 落下ブロックは後で処理
                if (TileID.Sets.Falling[tile.TileType])
                {
                    hasFallingBlock = true;
                    continue;
                }

                // 掘れないブロック
                if (!Helpers.MiningHelper.CanMine(tile.TileType,MiningLevel >=2))
                    continue;

                if (IsOre(tile.TileType))
                {
                    CurrentState = MinerState.OreMiningMode;
                    NPC.velocity.X = 0;
                    return;
                }

                ushort minedType = tile.TileType;

                WorldGen.KillTile(targetX, targetY);

                TryMiningBonusDrop(targetX, targetY, minedType);
                TryDropMiningCrate(targetX, targetY);

                Tile after = Framing.GetTileSafely(targetX, targetY);

                if (!after.HasTile)
                {
                    MinedBlockCount++;

                    if (MinedBlockCount >= MaxMineBlocks)
                    {
                        StopMining();
                        return;
                    }
                }

                // 1ブロックだけ掘って終了
                return;
            }

            // 通常ブロックは無いが落下ブロックがある
            if (hasFallingBlock)
            {
                fallingWaitTimer = 0;
                CurrentState = MinerState.FallingMining;
                return;
            }

            // 下・中・上すべて空なので前進
            advanceTargetX = NPC.Center.X + miningDirection * 16f;

            torchCounter++;

            if (torchCounter >= TorchInterval)
            {
                torchCounter = 0;
                TryPlaceTorch();
            }

            CurrentState = MinerState.Advancing;
        }

        private void MineFallingBlocks()
        {
            miningTimer++;

            if (miningTimer < GetMiningDelay())
            {
                isSwingingPickaxe = true;
                return;
            }

            miningTimer = 0;
            isSwingingPickaxe = false;

            int targetX = miningColumn.X;
            int targetY = miningColumn.Y;

            Tile tile = Framing.GetTileSafely(targetX, targetY);

            if (tile.HasTile &&
                TileID.Sets.Falling[tile.TileType])
            {
                WorldGen.KillTile(targetX, targetY);
                TryDropMiningCrate(targetX, targetY);

                Tile after = Framing.GetTileSafely(targetX, targetY);

                if (!after.HasTile)
                {
                    MinedBlockCount++;

                    if (MinedBlockCount >= MaxMineBlocks)
                    {
                        StopMining();
                        return;
                    }
                }
            }

            // 砂が落ちるまで待機
            fallingWaitTimer++;

            if (fallingWaitTimer < 8)
                return;

            fallingWaitTimer = 0;

            for (int y = miningColumn.Y - 2; y <= miningColumn.Y; y++)
            {
                Tile check = Framing.GetTileSafely(targetX, y);

                if (check.HasTile &&
                    TileID.Sets.Falling[check.TileType])
                {
                    return;
                }
            }

            CurrentState = MinerState.Mining;
        }

        private void MineNearbyOre()
        {
            if (!NPC.collideY)
            {
                isSwingingPickaxe = false;
                return;
            }
            Point? ore = FindNearbyOre();

            //鉱石なし
            if (ore == null)
            {
                CurrentState = MinerState.Mining;
                return;
            }

            Point targetOre = ore.Value;
            miningTimer++;

            if (miningTimer < GetMiningDelay())
            {
                isSwingingPickaxe = true;
                return;
            }

            miningTimer = 0;
            isSwingingPickaxe = false;

            Tile tile =
                Framing.GetTileSafely(
                    targetOre.X,
                    targetOre.Y
                );

            if (tile.HasTile && IsOre(tile.TileType)&&
                Helpers.MiningHelper.CanMine(tile.TileType,MiningLevel >= 2))
            {
                ushort type = tile.TileType;

                WorldGen.KillTile(
                    targetOre.X,
                    targetOre.Y
                );

                TryMiningBonusDrop(
                    targetOre.X,
                    targetOre.Y,
                    type
                );

                MinedBlockCount++;

                if (MinedBlockCount >= MaxMineBlocks)
                {
                    StopMining();
                }
            }
        }

        private Point? FindNearbyOre()
        {
            int centerX =
                (int)(NPC.Center.X / 16);

            int centerY =
                (int)(NPC.Center.Y / 16);

            Point? nearest = null;
            float nearestDistance = float.MaxValue;

            for (int x = -3; x <= 3; x++)
            {
                for (int y = -3; y <= 3; y++)
                {
                    Tile tile =
                        Framing.GetTileSafely(
                            centerX + x,
                            centerY + y
                        );

                    if (tile.HasTile &&
                        IsOre(tile.TileType)&&
                        Helpers.MiningHelper.CanMine(tile.TileType, MiningLevel >= 2))
                    {
                        float distance =
                            Vector2.Distance(
                                new Vector2(centerX, centerY),
                                new Vector2(centerX + x, centerY + y)
                            );

                        if (distance < nearestDistance)
                        {
                            nearestDistance = distance;
                            nearest =
                                new Point(
                                    centerX + x,
                                    centerY + y
                                );
                        }
                    }
                }
            }
            return nearest;
        }

        private bool CheckMiningColumn()
        {
            int x =
                (int)(NPC.Center.X / 16) + miningDirection;
            int y =
                (int)(NPC.Bottom.Y / 16) - 1;

            for (int i = 0; i < 3; i++)
            {
                Tile tile =
                    Framing.GetTileSafely(
                        x,
                        y - i
                    );

                if (tile.HasTile)
                {
                    if (Helpers.MiningHelper.CanMine(tile.TileType))
                        return true;
                }
            }
            return false;
        }

        //stop mining
        public void StopMining()
        {
            IsMining = false;

            CurrentState = MinerState.Idle;

            miningTimer = 0;
            isSwingingPickaxe = false;
            swingFrame = 0;

            NPC.velocity.X = 0;
        }

        //
        private void MoveMiningDirection()
        {
            NPC.direction = miningDirection;
            NPC.velocity.X = miningDirection * 2.5f;

            if (Vector2.Distance(NPC.Center, miningStartPosition) > MaxMiningTravel)
            {
                Main.NewText("壁が見つかりませんでした。");
                StopMining();
                return;
            }

            if (!CheckMiningColumn())
            {
                NPC.velocity.X = miningDirection * 2.5f;
                return;
            }

            // 壁の列を記録
            miningColumn = new Point(
                (int)(NPC.Center.X / 16) + miningDirection,
                (int)(NPC.Bottom.Y / 16) - 1
            );

            miningTimer = 0;

            NPC.velocity.X = 0;

            CurrentState = MinerState.Mining;
        }

        private void AdvanceMining()
        {
            NPC.direction = miningDirection;

            if (Math.Abs(NPC.Center.X - advanceTargetX) > 2f)
            {
                NPC.velocity.X = miningDirection * 2f;
                return;
            }

            NPC.velocity.X = 0;
            CurrentState = MinerState.MovingToMiningArea;
        }

        private void TryMiningBonusDrop(
            int x,
            int y,
            ushort tileType)
        {
            //Main.NewText($"TileType = {tileType}");
            //鉱石系だけ対象
            if (!IsOre(tileType))
                return;

            //30%発動
            if (Main.rand.Next(100) >= 30)
                return;

            int bonusAmount = MiningLevel >= 2 ? 4:1;

            Item.NewItem(
                null,
                new Rectangle(
                    x * 16,
                    y * 16,
                    16,
                    16
                ),
                GetOreItem(tileType),
                bonusAmount
            );
            SoundEngine.PlaySound(
                SoundID.ResearchComplete, new Vector2(x * 16 + 8, y * 16 + 8));

            CreateMiningBonusEffect(
                new Vector2(x * 16 + 8, y * 16 + 8)
            );
        }

        private bool IsOre(ushort tileType)
        {
            return tileType == TileID.Copper
                || tileType == TileID.Tin
                || tileType == TileID.Iron
                || tileType == TileID.Lead
                || tileType == TileID.Silver
                || tileType == TileID.Tungsten
                || tileType == TileID.Gold
                || tileType == TileID.Platinum
                || tileType == TileID.Demonite
                || tileType == TileID.Crimtane
                || tileType == TileID.Meteorite
                || tileType == TileID.Hellstone
                || tileType == TileID.Cobalt
                || tileType == TileID.Palladium
                || tileType == TileID.Mythril
                || tileType == TileID.Orichalcum
                || tileType == TileID.Titanium
                || tileType == TileID.Adamantite
                || tileType == TileID.Chlorophyte;
        }

        private int GetOreItem(ushort tileType)
        {
            switch (tileType)
            {
                case TileID.Copper:
                    return ItemID.CopperOre;

                case TileID.Tin:
                    return ItemID.TinOre;

                case TileID.Iron:
                    return ItemID.IronOre;

                case TileID.Lead:
                    return ItemID.LeadOre;

                case TileID.Silver:
                    return ItemID.SilverOre;

                case TileID.Tungsten:
                    return ItemID.TungstenOre;

                case TileID.Gold:
                    return ItemID.GoldOre;

                case TileID.Platinum:
                    return ItemID.PlatinumOre;

                case TileID.Demonite:
                    return ItemID.DemoniteOre;

                case TileID.Crimtane:
                    return ItemID.CrimtaneOre;

                case TileID.Meteorite:
                    return ItemID.MeteoriteBar; //後で修正推奨

                case TileID.Hellstone:
                    return ItemID.Hellstone;

                case TileID.Cobalt:
                    return ItemID.CobaltOre;
                case TileID.Palladium:
                    return ItemID.PalladiumOre;
                case TileID.Mythril:
                    return ItemID.MythrilOre;
                case TileID.Orichalcum:
                    return ItemID.OrichalcumOre;
                case TileID.Titanium:
                    return ItemID.TitaniumOre;
                case TileID.Adamantite:
                    return ItemID.AdamantiteOre;

                case TileID.Chlorophyte:
                    return ItemID.ChlorophyteOre;

                default:
                    return ItemID.StoneBlock;
            }
        }
        private void TryDropMiningCrate(int x, int y)
        {
            // 3%の確率
            if (Main.rand.NextFloat() > 0.03f)
                return;


            Item.NewItem(
                null,
                new Rectangle(
                    x * 16,
                    y * 16,
                    16,
                    16
                ),
                ModContent.ItemType<BuriedBox>()
            );
        }

        private void TryPlaceTorch()
        {
            Player player = Main.player[Main.myPlayer];

            if (!player.active)
                return;

            // プレイヤーのインベントリから通常の松明を探す
            int torchSlot = -1;

            for (int i = 0; i < player.inventory.Length; i++)
            {
                if (player.inventory[i].type == ItemID.Torch &&
                    player.inventory[i].stack > 0)
                {
                    torchSlot = i;
                    break;
                }
            }

            // 松明が無い
            if (torchSlot == -1)
                return;

            int x = (int)(NPC.Center.X / 16);
            int y = (int)(NPC.Bottom.Y / 16);

            // 足元がブロックで、その上が空いているなら設置
            if (!Main.tile[x, y].HasTile)
                return;

            if (Main.tile[x, y - 1].HasTile)
                return;

            if (WorldGen.PlaceTile(x, y - 1, TileID.Torches))
            {
                player.inventory[torchSlot].stack--;

                if (player.inventory[torchSlot].stack <= 0)
                    player.inventory[torchSlot].TurnToAir();
            }
        }

        //chat
        public override bool CanChat()
        {
            return true;
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            if (IsMining)
            {
                button = "採掘停止";
            }
            else
            {
                button = "採掘開始";
            }

            button2 = "休ませる";
        }

        public override void OnChatButtonClicked(bool firstButton, ref string shopName)
        {
            if (firstButton)
            {
                if (IsMining)
                {
                    StopMining();
                    Main.NewText("いったん休憩するよ。");
                }
                else
                {
                    StartMining();
                    Main.NewText("わかった、採掘を始めよう。");
                }
                return;
            }

            Rest();

        }

        public override string GetChat()
        {
            if (IsMining)
                return "採掘中です。止めますか？";

            return "いつでも採掘できます。";
        }

        //animation
        public override void FindFrame(int frameHeight)
        {
            // 採掘モーション
            if (isSwingingPickaxe)
            {
                NPC.frameCounter++;

                if (NPC.frameCounter >= 5)
                {
                    NPC.frameCounter = 0;

                    swingFrame++;

                    // 仮の採掘フレーム
                    // 後で専用画像に差し替え
                    NPC.frame.Y = frameHeight * (17 + swingFrame);

                    if (swingFrame >= 4)
                    {
                        swingFrame = 0;
                    }
                }

                return;
            }


            // 空中
            if (!NPC.collideY)
            {
                NPC.frame.Y = frameHeight * 15;
                return;
            }


            // 歩行
            if (System.Math.Abs(NPC.velocity.X) > 0.1f)
            {
                NPC.frameCounter++;

                if (NPC.frameCounter >= 6)
                {
                    NPC.frameCounter = 0;

                    NPC.frame.Y += frameHeight;

                    if (NPC.frame.Y < frameHeight * 7 ||
                        NPC.frame.Y > frameHeight * 15)
                    {
                        NPC.frame.Y = frameHeight * 7;
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

                    if (NPC.frame.Y > frameHeight * 7)
                        NPC.frame.Y = 0;
                }
            }
        }

        private void DrawPickaxe(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            if (!isSwingingPickaxe || miningTool == null || miningTool.IsAir)
                return;

            Texture2D texture = TextureAssets.Item[miningTool.type].Value;

            SpriteEffects effects = NPC.spriteDirection == -1
                ? SpriteEffects.FlipHorizontally
                : SpriteEffects.None;

            // ガイド系NPCの右手付近
            Vector2 handOffset = new Vector2(8f * NPC.direction, -12f);
            Vector2 drawPos = NPC.Center + handOffset - screenPos;

            // 柄の付け根を回転中心にする
            Vector2 origin = new Vector2(texture.Width * 0.2f, texture.Height * 0.8f);

            float rotation;

            if (NPC.spriteDirection == 1)
            {
                rotation = MathHelper.ToRadians(-90f + swingFrame * 35f);
            }
            else
            {
                rotation = MathHelper.ToRadians(270f - swingFrame * 35f);
            }

            spriteBatch.Draw(
                texture,
                drawPos,
                null,
                Lighting.GetColor((int)NPC.Center.X / 16, (int)NPC.Center.Y / 16),
                rotation,
                origin,
                1f,
                effects,
                0f);
        }

        private void CreateMiningBonusEffect(Vector2 position)
        {
            for (int i = 0; i < 12; i++)
            {
                Dust dust = Dust.NewDustPerfect(
                    position,
                    DustID.GoldFlame,
                    Main.rand.NextVector2Circular(2f, 2f)
                );

                dust.noGravity = true;
                dust.scale = 1.2f;
            }

            for (int i = 0; i < 6; i++)
            {
                Dust.NewDustPerfect(
                    position,
                    DustID.WhiteTorch,
                    Main.rand.NextVector2Circular(3f, 3f)
                ).noGravity = true;
            }

            CombatText.NewText(
                new Rectangle(
                    (int)position.X,
                    (int)position.Y,
                    16,
                    16
                ),
                Color.Gold,
                "Lucky!"
            );
        }

        public override void PostDraw(
            SpriteBatch spriteBatch,
            Vector2 screenPos,
            Color drawColor)
        {
            DrawPickaxe(spriteBatch, screenPos);
        }

        public void Rest()
        {
            IsMining = false;
            CurrentState = MinerState.Idle;

            Main.NewText($"{NPC.GivenName} : また呼んでくれよ！", 255, 255, 0);

            NPC.active = false;
        }
    }
}