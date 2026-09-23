using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace MinerManagementMOD.Tiles
{
    public class MinerOutpost : ModTile
    {
        public override void SetStaticDefaults()
        {
            // ==========================================
            // 基本設定
            // ==========================================

            Main.tileFrameImportant[Type] = true;
            Main.tileSolid[Type] = false;
            Main.tileNoAttach[Type] = true;

            DustType = DustID.Iron;


            // ==========================================
            // 2 × 4 タイル
            //
            // 横32px = 2タイル
            // 縦64px = 4タイル
            // ==========================================

            TileObjectData.newTile.CopyFrom(
                TileObjectData.Style2xX
            );

            TileObjectData.newTile.Width = 2;
            TileObjectData.newTile.Height = 4;

            TileObjectData.newTile.CoordinateHeights =
                new int[]
                {
                    16,
                    16,
                    16,
                    16
                };

            TileObjectData.addTile(Type);


            // ==========================================
            // マップ表示
            // ==========================================

            AddMapEntry(
                new Color(90, 130, 90),
                CreateMapEntryName()
            );
        }


        // ==========================================
        // Outpost設置
        // ==========================================

        public override void PlaceInWorld(
            int i,
            int j,
            Item item)
        {
            base.PlaceInWorld(i, j, item);

            Tile tile = Main.tile[i, j];

            // マルチタイルの左上座標を取得
            int left =
                i - tile.TileFrameX / 16;

            int top =
                j - tile.TileFrameY / 16;

            Systems.MinerStationSystem.RegisterOutpost(
                left,
                top
            );
        }


        // ==========================================
        // Outpost破壊
        // ==========================================

        public override void KillTile(
            int i,
            int j,
            ref bool fail,
            ref bool effectOnly,
            ref bool noItem)
        {
            // 破壊前に左上座標を取得
            Tile tile = Main.tile[i, j];

            int left =
                i - tile.TileFrameX / 16;

            int top =
                j - tile.TileFrameY / 16;


            base.KillTile(
                i,
                j,
                ref fail,
                ref effectOnly,
                ref noItem
            );


            if (!fail && !effectOnly)
            {
                Systems.MinerStationSystem.UnregisterOutpost(
                    left,
                    top
                );
            }
        }


        // ==========================================
        // 右クリック
        // ==========================================

        public override bool RightClick(int i, int j)
        {
            // ==========================================
            // Miner Baseが存在するか確認
            // ==========================================

            Point16 basePosition =
                Systems.MinerStationSystem.MinerBasePosition;

            if (basePosition.X < 0 ||
                basePosition.Y < 0)
            {
                return true;
            }


            // ==========================================
            // Miner Baseの中央へテレポート
            //
            // Baseは2 × 4タイル
            //
            // 左上から中央まで
            // X = +16px
            // Y = +32px
            // ==========================================

            Vector2 teleportPosition =
                new Vector2(
                    basePosition.X * 16f + 16f,
                    basePosition.Y * 16f + 32f
                );


            // ==========================================
            // テレポート
            // ==========================================

            Main.LocalPlayer.Teleport(
                teleportPosition,
                0
            );


            return true;
        }
    }
}