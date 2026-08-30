using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MinerManagementMOD.Items
{
    public class DrillRocket2 : ModItem
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

            Item.value = Item.buyPrice(gold: 30);
            Item.rare = ItemRarityID.Blue;
        }

        public override void AddRecipes()
        {
            CreateRecipe(50)
                .AddRecipeGroup("MythrilOrOrichalcumBar",1)
                .AddIngredient<DrillRocket>(50)
                .AddIngredient<GoldMinerCoin>(6)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}