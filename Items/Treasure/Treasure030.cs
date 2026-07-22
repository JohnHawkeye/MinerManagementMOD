using Terraria;
using Terraria.ID;

namespace MinerManagementMOD.Items.Treasure
{
    public class Treasure030 : TreasureBase
    {
        public override int TreasureID => 30;

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.value = Item.buyPrice(copper: Main.rand.Next(40, 60));
            Item.rare = ItemRarityID.White;
        }
    }
}
