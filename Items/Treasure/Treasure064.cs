using Terraria;
using Terraria.ID;

namespace MinerManagementMOD.Items.Treasure
{
    public class Treasure064 : TreasureBase
    {
        public override int TreasureID => 64;

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.value = Item.buyPrice(silver: Main.rand.Next(10, 30));
            Item.rare = ItemRarityID.Blue;
        }
    }
}
