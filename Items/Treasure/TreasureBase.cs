using Terraria;
using Terraria.ModLoader;

namespace MinerManagementMOD.Items.Treasure
{
    public abstract class TreasureBase : ModItem
    {
        /// <summary>
        /// トレジャーID
        /// </summary>
        public abstract int TreasureID { get; }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;

            Item.maxStack = 1;

            Item.value = Item.buyPrice(gold: 1);

            Item.rare = Terraria.ID.ItemRarityID.White;
        }
    }
}