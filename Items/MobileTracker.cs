using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MinerManagementMOD.Items
{
    public class MobileTracker : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;

            Item.maxStack = 1;

            Item.value = Item.buyPrice(gold: 10);
            Item.rare = ItemRarityID.Green;

            // 消耗品ではなく、所持しているだけで効果を発揮
            Item.accessory = false;
        }

    }
}