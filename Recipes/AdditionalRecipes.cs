using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MinerManagementMOD.Recipes
{
    public class AdditionalRecipes : ModSystem
    {
        public override void AddRecipes()
        {
            // ヒールポーション
            Recipe.Create(ItemID.HealingPotion, 100)
                .AddIngredient(ItemID.LifeCrystal, 1)
                .AddIngredient(ItemID.Gel, 20)
                .AddIngredient(ItemID.BottledWater, 100)
                .AddTile(TileID.Bottles)
                .Register();

            // マナポーション
            Recipe.Create(ItemID.ManaPotion, 100)
                .AddIngredient(ItemID.ManaCrystal, 1)
                .AddIngredient(ItemID.Gel, 20)
                .AddIngredient(ItemID.BottledWater, 100)
                .AddTile(TileID.Bottles)
                .Register();

            // Mining Helmet
            Recipe.Create(ItemID.MiningHelmet)
                .AddIngredient(ItemID.IronBar, 10)
                .AddIngredient(ItemID.Topaz, 1)
                .AddIngredient(ItemID.Torch, 5)
                .AddTile(TileID.Anvils)
                .Register();

            // Mining Shirt
            Recipe.Create(ItemID.MiningShirt)
                .AddIngredient(ItemID.Silk, 10)
                .AddIngredient(ItemID.IronBar, 6)
                .AddTile(TileID.Loom)
                .Register();

            // Mining Pants
            Recipe.Create(ItemID.MiningPants)
                .AddIngredient(ItemID.Silk, 8)
                .AddIngredient(ItemID.IronBar, 4)
                .AddTile(TileID.Loom)
                .Register();
        }
    }
}