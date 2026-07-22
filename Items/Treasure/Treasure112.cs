using Terraria;
using Terraria.ID;

namespace MinerManagementMOD.Items.Treasure
{
    public class Treasure112 : TreasureBase
    {
        public override int TreasureID => 112;

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.value = Item.buyPrice(gold: Main.rand.Next(20, 30));
            Item.rare = ItemRarityID.Orange;
        }
    }
}
