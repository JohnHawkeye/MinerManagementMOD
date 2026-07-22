using Terraria;
using Terraria.ID;

namespace MinerManagementMOD.Items.Treasure
{
    public class Treasure072 : TreasureBase
    {
        public override int TreasureID => 72;

        public override void SetDefaults()
        {
            base.SetDefaults();
            
            Item.value = Item.buyPrice(silver: Main.rand.Next(20, 40));
            Item.rare = ItemRarityID.Blue;
        }
    }
}
