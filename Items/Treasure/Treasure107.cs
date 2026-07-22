using Terraria;
using Terraria.ID;

namespace MinerManagementMOD.Items.Treasure
{
    public class Treasure107 : TreasureBase
    {
        public override int TreasureID => 107;

        public override void SetDefaults()
        {
            base.SetDefaults();
            
            Item.value = Item.buyPrice(gold: Main.rand.Next(10, 20));
            Item.rare = ItemRarityID.Orange;
        }
    }
}
