using Terraria;
using Terraria.ID;

namespace MinerManagementMOD.Items.Treasure
{
    public class Treasure124 : TreasureBase
    {
        public override int TreasureID => 124;

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.value = Item.buyPrice(gold: Main.rand.Next(70, 80));
            Item.rare = ItemRarityID.LightRed;
        }
    }
}
