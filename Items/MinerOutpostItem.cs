using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MinerManagementMOD.Items
{
    public class MinerOutpostItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;

            Item.maxStack = 99;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 10;
            Item.useAnimation = 15;

            Item.useTurn = true;
            Item.autoReuse = true;

            Item.consumable = true;

            Item.createTile = ModContent.TileType<Tiles.MinerOutpost>();
        }
        
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SilverMinerCoin>(10)
                .AddRecipeGroup("IronOrLeadBar",5)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}