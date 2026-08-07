using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.Audio;
using System;

namespace MinerManagementMOD.Players
{
    public class LockPickingPlayer : ModPlayer
    {
        public bool IsPicking;

        public int ChestX;
        public int ChestY;

        public int SuccessCount;
        public float SuccessStartAngle = 90f;
        public float SuccessEndAngle = 180f;
        public const float SuccessRange = 60f;
        public float NeedleAngle;
        public float NeedleSpeed = 3f;
        public int ChestIndex = -1;

        public override void PreUpdate()
        {
            if (IsPicking)
            {
                NeedleAngle += NeedleSpeed;

                if (NeedleAngle >= 360f)
                    NeedleAngle -= 360f;
            }

            if (Main.mouseLeft && Main.mouseLeftRelease)
            {
                if (IsPicking)
                {
                    CheckPick();
                }
                else
                {
                    TryStartPicking();
                }

            }
        }

        private void TryStartPicking()
        {

            if (Player.HeldItem.type != ModContent.ItemType<Items.LockPick>())
            {
                //Main.NewText("ピッキングツールを持っていません");
                return;
            }

            Point tile =
                Main.MouseWorld.ToTileCoordinates();

            Tile tile2 = Main.tile[tile.X, tile.Y];

            int chestX = tile.X;
            int chestY = tile.Y;

            // チェスト左上へ補正
            Tile target = Main.tile[chestX, chestY];

            if (target != null)
            {
                chestX -= target.TileFrameX / 18 % 2;
                chestY -= target.TileFrameY / 18 % 2;
            }

            if (IsLockedGoldChest(chestX, chestY))
            {
                StartPicking(chestX, chestY);
            }
        }

        private bool IsLockedGoldChest(int x, int y)
        {
            Tile tile = Main.tile[x, y];

            if (tile == null)
                return false;

            if (tile.TileType != TileID.Containers)
                return false;

            // チェスト左上へ補正
            int chestX = x - tile.TileFrameX / 18 % 2;
            int chestY = y - tile.TileFrameY / 18 % 2;


            Tile chestTile = Main.tile[chestX, chestY];

            // ゴールドチェスト判定
            int style = chestTile.TileFrameX / 36;

            if (style == 2)
            {
                return true;
            }
            return false;
        }

        public override void ResetEffects()
        {
            if (!IsPicking)
                return;


            // ピッキング中は他アイテム使用不可
            Player.itemAnimation = 0;
            Player.itemTime = 0;


            // チェストから離れたらキャンセル
            Vector2 chestPos =
                new Vector2(
                    ChestX * 16 + 16,
                    ChestY * 16 + 16
                );


            if (Vector2.Distance(Player.Center, chestPos) > 80f)
            {
                CancelPicking();
            }
        }


        public override bool CanUseItem(Item item)
        {
            if (IsPicking)
                return false;

            return base.CanUseItem(item);
        }

        public void RandomizeSuccessRange()
        {
            SuccessStartAngle = Main.rand.NextFloat(0f, 360f);
            SuccessEndAngle = SuccessStartAngle + SuccessRange;
        }

        public void StartPicking(int x, int y)
        {
            IsPicking = true;

            ChestX = x;
            ChestY = y;

            SuccessCount = 0;
            NeedleAngle = 0f;

            RandomizeSuccessRange();

            Main.NewText("ピッキングを開始しました。");
        }

        private void CheckPick()
        {

            if (IsSuccessAngle(NeedleAngle))
            {
                SuccessCount++;

                SoundEngine.PlaySound(
                                   SoundID.ResearchComplete,
                                   Main.LocalPlayer.Center);

                Main.NewText("成功！ " + SuccessCount + "/3");

                RandomizeSuccessRange();

                if (SuccessCount >= 3)
                {
                    UnlockChest();
                }
            }
            else
            {
                SoundEngine.PlaySound(
                    SoundID.PlayerHit,
                    Player.Center
                );

                Main.NewText("失敗してしまった…");

                CancelPicking();
            }
        }

        private bool IsSuccessAngle(float angle)
        {
            return angle >= SuccessStartAngle &&
                   angle <= SuccessStartAngle + SuccessRange;
        }

        public void CancelPicking()
        {
            if (!IsPicking)
                return;

            IsPicking = false;

            Main.NewText("ピッキングを中断しました。");
        }

        public void UnlockChest()
        {
            if (ChestX < 0 || ChestY < 0)
                return;

            Tile tile = Main.tile[ChestX, ChestY];

            if (tile == null)
                return;

            // チェスト左上へ補正
            int x = ChestX;
            int y = ChestY;

            while (x > 0 && Main.tile[x, y].TileFrameX % 36 != 0)
            {
                x--;
            }

            // ロック解除
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Tile chestTile = Main.tile[x + i, y + j];

                    chestTile.TileFrameX -= 36;
                }
            }

            WorldGen.SquareTileFrame(
                x,
                y
            );

            SoundEngine.PlaySound(
                SoundID.Unlock,
                Player.Center);

            for (int i = 0; i < 20; i++)
            {
                Dust.NewDust(
                    Player.position,
                    Player.width,
                    Player.height,
                    DustID.GemTopaz,
                    Main.rand.NextFloat(-3f, 3f),
                    Main.rand.NextFloat(-3f, 3f)
                );
            }

            CombatText.NewText(
                Player.Hitbox,
                Color.Gold,
                "UNLOCK!"
            );

            IsPicking = false;
        }
    }
}