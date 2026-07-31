using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MinerManagementMOD.Items
{
    public class SilverMinerCoin : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 9999;
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;

            Item.maxStack = 999;

            // 銅貨1枚と同じ価値
            Item.value = Item.buyPrice(silver: 1);

            Item.rare = ItemRarityID.White;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SilverCoin)
                .Register();

            CreateRecipe(100)
                .AddIngredient(ItemID.GoldCoin)
                .Register();
        }
    }
}