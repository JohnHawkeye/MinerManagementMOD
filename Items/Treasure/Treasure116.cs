using Terraria;
using Terraria.ID;

namespace MinerManagementMOD.Items.Treasure
{
    public class Treasure116 : TreasureBase
    {
        public override int TreasureID => 116;

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.value = Item.buyPrice(gold: Main.rand.Next(30, 40));
            Item.rare = ItemRarityID.Orange;
        }
    }
}
