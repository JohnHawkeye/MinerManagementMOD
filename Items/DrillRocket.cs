using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MinerManagementMOD.Items
{
    public class DrillRocket : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 99;
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;

            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;

            // このアイテム自身を弾薬にする
            Item.ammo = Item.type;

            Item.value = Item.buyPrice(silver: 1);
            Item.rare = ItemRarityID.Blue;
        }

        public override void AddRecipes()
        {
            CreateRecipe(50)
                .AddRecipeGroup(RecipeGroupID.IronBar, 10)
                .AddIngredient(ItemID.Wire, 10)
                .AddIngredient(ItemID.Gel, 10)
                .AddIngredient<SilverMinerCoin>(50)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}