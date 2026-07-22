using Terraria;
using Terraria.ID;

namespace MinerManagementMOD.Items.Treasure
{
    public class Treasure119 : TreasureBase
    {
        public override int TreasureID => 119;

        public override void SetDefaults()
        {
            base.SetDefaults();


            Item.value = Item.buyPrice(gold: Main.rand.Next(40, 50));
            Item.rare = ItemRarityID.Orange;
        }
    }
}
