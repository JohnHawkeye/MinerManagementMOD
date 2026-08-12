using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MinerManagementMOD.Items
{
    public class FoolTicket : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;

            Item.maxStack = 999;

            Item.value = 0;
            Item.rare = ItemRarityID.White;

            Item.consumable = false;
        }

        public override void AddRecipes()
        {
            // 作成不可
        }
    }
}