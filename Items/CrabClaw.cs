using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MinerManagementMOD.Items
{
    public class CrabClaw : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;

            Item.maxStack = 999;

            Item.value = Item.buyPrice(silver: 20);

            Item.rare = ItemRarityID.Blue;
        }

        public override void AddRecipes()
        {
            // 必要ならここに加工レシピを追加
        }
    }
}