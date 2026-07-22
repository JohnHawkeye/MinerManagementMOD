using Terraria;
using Terraria.ID;

namespace MinerManagementMOD.Items.Treasure
{
    public class Treasure118 : TreasureBase
    {
        public override int TreasureID => 118;

        public override void SetDefaults()
        {
            base.SetDefaults();


            Item.value = Item.buyPrice(gold: Main.rand.Next(40, 50));
            Item.rare = ItemRarityID.Orange;
        }
    }
}
