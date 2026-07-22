using Terraria;
using Terraria.ID;

namespace MinerManagementMOD.Items.Treasure
{
    public class Treasure117 : TreasureBase
    {
        public override int TreasureID => 117;

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.value = Item.buyPrice(gold: Main.rand.Next(40, 50));
            Item.rare = ItemRarityID.Orange;
        }
    }
}
