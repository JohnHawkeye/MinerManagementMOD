using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;

namespace MinerManagementMOD.Items
{
    public class SlopeAccessory : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 28;

            Item.accessory = true;

            // 仮の売却価格
            Item.value = Item.buyPrice(silver: 50);

            Item.rare = ItemRarityID.Blue;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            MakeSlope(player);
        }

        private void MakeSlope(Player player)
        {
            // プレイヤーの足元中央のTile座標
            int centerX = (int)(player.Center.X / 16f);
            int footY = (int)(player.Bottom.Y / 16f);

            // 中央のブロック
            int centerY = footY;

            // 左・中央・右
            Tile leftTile = Main.tile[centerX - 1, centerY];
            Tile centerTile = Main.tile[centerX, centerY];
            Tile rightTile = Main.tile[centerX + 1, centerY];

            // 3つとも有効なTileか確認
            if (leftTile == null ||
                centerTile == null ||
                rightTile == null)
            {
                return;
            }

            // 中央が存在しないなら何もしない
            if (!centerTile.HasTile)
            {
                return;
            }

            // helper だけを対象にする
            if (!Helpers.MiningHelper.CanMine(centerTile.TileType))
            {
                return;
            }

            // -------------------------------------------------
            // □ ■ ■
            //   ↑
            // 中央を SlopeDownRight
            // -------------------------------------------------

            if (!leftTile.HasTile &&
                centerTile.HasTile &&
                rightTile.HasTile)
            {
                centerTile.Slope = SlopeType.SlopeDownRight;

                WorldGen.SquareTileFrame(
                    centerX,
                    centerY,
                    true
                );

                return;
            }

            // -------------------------------------------------
            // ■ ■ □
            //   ↑
            // 中央を SlopeDownLeft
            // -------------------------------------------------

            if (leftTile.HasTile &&
                centerTile.HasTile &&
                !rightTile.HasTile)
            {
                centerTile.Slope = SlopeType.SlopeDownLeft;

                WorldGen.SquareTileFrame(
                    centerX,
                    centerY,
                    true
                );

                return;
            }
        }
    }
}