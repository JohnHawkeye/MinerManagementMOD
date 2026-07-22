using Terraria;
using Terraria.ID;

namespace MinerManagementMOD.Items.Treasure
{
    public class Treasure046 : TreasureBase
    {
        public override int TreasureID => 46;

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.value = Item.buyPrice(copper: Main.rand.Next(80,100));
            Item.rare = ItemRarityID.White;
        }
    }
}
