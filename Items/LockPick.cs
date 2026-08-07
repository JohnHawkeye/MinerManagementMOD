using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;
using MinerManagementMOD.Players;

namespace MinerManagementMOD.Items
{
    public class LockPick : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;

            Item.useTime = 10;
            Item.useAnimation = 10;

            Item.useStyle = ItemUseStyleID.Swing;

            Item.noMelee = true;
            Item.autoReuse = false;
        }


        public override bool CanUseItem(Player player)
        {
            Point tile = Main.MouseWorld.ToTileCoordinates();

            int chestIndex =
                Chest.FindChest(
                    tile.X,
                    tile.Y
                );


            if (chestIndex >= 0)
            {
                Chest chest =
                    Main.chest[chestIndex];


                if (chest != null)
                {
                    // 仮
                    if (IsLockedGoldChest(
                        tile.X,
                        tile.Y))
                    {
                        player
                        .GetModPlayer<LockPickingPlayer>()
                        .StartPicking(
                            tile.X,
                            tile.Y
                        );


                        return false;
                    }
                }
            }


            return false;
        }

        private bool IsLockedGoldChest(
            int x,
            int y)
        {
            Tile tile =
                Main.tile[x, y];

            if (tile == null)
                return false;

            if (tile.TileType != TileID.Containers)
                return false;

            int style =
                tile.TileFrameX / 36;

            // ゴールドチェスト
            // ※フレーム値はtModLoader環境で確認調整

            if (style == 1)
            {
                return true;
            }

            return false;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();

            recipe.AddIngredient(ItemID.IronBar, 1);
            recipe.AddIngredient<GoldMinerCoin>(1);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}