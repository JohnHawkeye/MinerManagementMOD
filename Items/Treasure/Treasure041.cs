using Terraria;
using Terraria.ID;

namespace MinerManagementMOD.Items.Treasure
{
    public class Treasure041 : TreasureBase
    {
        public override int TreasureID => 41;

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.value = Item.buyPrice(copper: Main.rand.Next(80, 100));
            Item.rare = ItemRarityID.White;
        }
    }
}
