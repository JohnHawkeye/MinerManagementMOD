using Terraria;
using Terraria.ID;

namespace MinerManagementMOD.Items.Treasure
{
    public class Treasure076 : TreasureBase
    {
        public override int TreasureID => 76;

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.value = Item.buyPrice(silver: Main.rand.Next(30, 50));
            Item.rare = ItemRarityID.Blue;
        }
    }
}
