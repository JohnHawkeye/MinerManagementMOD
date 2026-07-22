using Terraria;
using Terraria.ID;

namespace MinerManagementMOD.Items.Treasure
{
    public class Treasure099 : TreasureBase
    {
        public override int TreasureID => 99;

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.value = Item.buyPrice(silver: Main.rand.Next(80, 100));
            Item.rare = ItemRarityID.Green;
        }
    }
}
