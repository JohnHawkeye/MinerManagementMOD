using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using MinerManagementMOD.NPCs;

namespace MinerManagementMOD.Items
{
    public class CopperDiamondRing : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;

            Item.maxStack = 999;

            Item.value = Item.sellPrice(silver: 60);
            Item.rare = ItemRarityID.Blue;

            // 使用不可
            Item.useStyle = ItemUseStyleID.None;
            Item.useTime = 0;
            Item.useAnimation = 0;
            Item.consumable = false;
        }
        
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.CopperBar, 1)
                .AddIngredient(ItemID.Diamond, 1)
                .AddTile(TileID.Anvils)
                .Register();

            CreateRecipe()
                .AddIngredient(ItemID.TinBar, 1)
                .AddIngredient(ItemID.Diamond, 1)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}