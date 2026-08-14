using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MinerManagementMOD.Common
{
    public class MMMGlobalItem : GlobalItem
    {
        public override void SetDefaults(Item item)
        {
            // 黒曜石
            if (item.type == ItemID.Obsidian)
            {
                // NPCへの売却価格を1 Silverにする
                // Item.value は売却価格の5倍
                item.value = Item.buyPrice(silver: 5);
            }
        }
    }
}