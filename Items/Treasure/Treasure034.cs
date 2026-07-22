using Terraria;
using Terraria.ID;

namespace MinerManagementMOD.Items.Treasure
{
    public class Treasure034 : TreasureBase
    {
        public override int TreasureID => 34;

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.value = Item.buyPrice(copper: Main.rand.Next(60, 80));
            Item.rare = ItemRarityID.White;
        }
    }
}
