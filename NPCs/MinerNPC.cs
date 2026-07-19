using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace MinerManagementMOD.NPCs
{
    [AutoloadHead]
    public class MinerNPC : ModNPC
    {
        //data paras
        public int MinerID;
        public string MinerName;

        public int MiningPower;
        public int MiningSpeed;
        public int CarryCapacity;

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
        private int miningHeight = 0;
        private int miningTimer;
        private float advanceTargetX;

        public int MinedBlockCount = 0;
        private const int MaxMineBlocks = 150;

        public enum MinerState
        {
            Idle,
            MovingToMiningArea,
            Mining,
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

            // 上 → 中 → 下
            int targetY = miningColumn.Y - 2 + miningHeight;

            Tile tile = Framing.GetTileSafely(targetX, targetY);

            if (tile.HasTile &&
                Helpers.MiningHelper.CanMine(tile.TileType))
            {
                WorldGen.KillTile(targetX, targetY);

                Tile tileAfter = Framing.GetTileSafely(targetX, targetY);

                if (!tileAfter.HasTile)
                {
                    MinedBlockCount++;
                    if (MinedBlockCount >= MaxMineBlocks)
                    {
                        IsMining = false;
                        CurrentState = MinerState.Idle;

                        miningHeight = 0;
                        miningTimer = 0;
                        isSwingingPickaxe = false;

                        NPC.velocity = Vector2.Zero;
                        return;
                    }

                    miningHeight++;
                }
            }
            else
            {
                miningHeight++;
            }

            // 3ブロック掘ったら前進
            if (miningHeight >= 3)
            {
                miningHeight = 0;

                advanceTargetX = NPC.Center.X + miningDirection * 16;

                CurrentState = MinerState.Advancing;
            }
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

            if (!NPC.collideX)
                return;

            // 壁の列を記録
            miningColumn = new Point(
                (int)(NPC.Center.X / 16) + miningDirection,
                (int)(NPC.Bottom.Y / 16) - 1
            );

            miningHeight = 0;
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