using Terraria;
using Terraria.ID;

namespace MinerManagementMOD.Items.Treasure
{
    public class Treasure115 : TreasureBase
    {
        public override int TreasureID => 115;

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.value = Item.buyPrice(gold: Main.rand.Next(30, 40));
            Item.rare = ItemRarityID.Orange;
        }
    }
}
