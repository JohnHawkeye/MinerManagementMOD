using Terraria;
using Terraria.ID;

namespace MinerManagementMOD.Items.Treasure
{
    public class Treasure039 : TreasureBase
    {
        public override int TreasureID => 39;

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.value = Item.buyPrice(copper: Main.rand.Next(60, 80));
            Item.rare = ItemRarityID.White;
        }
    }
}
