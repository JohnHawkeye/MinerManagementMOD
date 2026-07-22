using Terraria;
using Terraria.ID;

namespace MinerManagementMOD.Items.Treasure
{
    public class Treasure085 : TreasureBase
    {
        public override int TreasureID => 85;

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.value = Item.buyPrice(silver: Main.rand.Next(60, 80));
            Item.rare = ItemRarityID.Green;
        }
    }
}
