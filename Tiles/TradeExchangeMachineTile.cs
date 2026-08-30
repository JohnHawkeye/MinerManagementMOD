using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Enums;

namespace MinerManagementMOD.Tiles
{
    public class TradeExchangeMachineTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;

            // ==========================================
            // 4 × 4 タイル
            // 64 × 64 pixel
            // ==========================================

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);

            TileObjectData.newTile.Width = 4;
            TileObjectData.newTile.Height = 4;

            TileObjectData.newTile.CoordinateWidth = 16;

            TileObjectData.newTile.CoordinateHeights = new int[]
            {
                16,
                16,
                16,
                16
            };

            TileObjectData.newTile.CoordinatePadding = 2;

            // 設置時の基準位置
            TileObjectData.newTile.Origin = new Point16(1, 3);

            // 床に設置
            TileObjectData.newTile.AnchorBottom =
                new AnchorData(
                    AnchorType.SolidTile |
                    AnchorType.SolidWithTop,
                    4,
                    0
                );

            TileObjectData.addTile(Type);

            // マップ上の色
            AddMapEntry(
                new Color(180, 180, 180),
                CreateMapEntryName()
            );

            DustType = DustID.Iron;
        }


        // ==========================================
        // マウスオーバー
        // ==========================================

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;

            player.noThrow = 2;

            player.cursorItemIconEnabled = true;
            player.cursorItemIconID =
                ModContent.ItemType<Items.TradeExchangeMachine>();
        }


        // ==========================================
        // 右クリック
        // ==========================================

        public override bool RightClick(int i, int j)
        {
            // 特別なUIは使用しない
            return false;
        }
    }
}