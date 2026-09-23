using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace MinerManagementMOD.Tiles
{
    public class MinerBase : ModTile
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
                new Color(120, 120, 120),
                CreateMapEntryName()
            );
        }


        // ==========================================
        // 設置
        // ==========================================

        public override void PlaceInWorld(
            int i,
            int j,
            Item item)
        {
            base.PlaceInWorld(i, j, item);

            // 設置されたタイルの情報を取得
            Tile tile = Main.tile[i, j];

            // マルチタイルの左上座標を取得
            int left = i - tile.TileFrameX / 16;
            int top = j - tile.TileFrameY / 16;

            Systems.MinerStationSystem.RegisterBase(
                left,
                top
            );
        }


        // ==========================================
        // 破壊
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

            int left = i - tile.TileFrameX / 16;
            int top = j - tile.TileFrameY / 16;


            base.KillTile(
                i,
                j,
                ref fail,
                ref effectOnly,
                ref noItem
            );


            // ==========================================
            // MinerBase登録解除
            // ==========================================

            if (!fail && !effectOnly)
            {
                Systems.MinerStationSystem.UnregisterBase(
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
            // Outpost選択モード開始
            // ==========================================

            Systems.MinerStationSystem.SelectingOutpost = true;


            // ==========================================
            // ワールドマップを開く
            // ==========================================

            Main.LocalPlayer.TryOpeningFullscreenMap();


            return true;
        }
    }
}