using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MinerManagementMOD.Items
{
    public class HoppingTrackItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;

            Item.maxStack = 9999;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 10;
            Item.useAnimation = 15;
            Item.autoReuse = true;

            Item.consumable = true;

            // ★ TileではなくWallを設置する
            Item.createWall = ModContent.WallType<Tiles.HoppingTrack>();

            Item.rare = ItemRarityID.Blue;

            Item.value = Item.buyPrice(
                silver: 50
            );
        }

        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ItemID.MinecartTrack, 1)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}