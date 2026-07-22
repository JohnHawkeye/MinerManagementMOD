using Terraria;
using Terraria.ID;

namespace MinerManagementMOD.Items.Treasure
{
    public class Treasure103 : TreasureBase
    {
        public override int TreasureID => 103;

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.value = Item.buyPrice(silver: Main.rand.Next(80, 100));
            Item.rare = ItemRarityID.Green;
        }
    }
}
