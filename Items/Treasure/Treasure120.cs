using Terraria;
using Terraria.ID;

namespace MinerManagementMOD.Items.Treasure
{
    public class Treasure120 : TreasureBase
    {
        public override int TreasureID => 120;

        public override void SetDefaults()
        {
            base.SetDefaults();


            Item.value = Item.buyPrice(gold: Main.rand.Next(40, 50));
            Item.rare = ItemRarityID.Orange;
        }
    }
}
