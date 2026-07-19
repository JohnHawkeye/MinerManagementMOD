using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using MinerManagementMOD.NPCs;

namespace MinerManagementMOD.Items
{
    public class CopperSword100 : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;

            Item.maxStack = 999;

            Item.value = Item.buyPrice(silver: 10);
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
                .AddIngredient(ItemID.CopperBar, 600)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}