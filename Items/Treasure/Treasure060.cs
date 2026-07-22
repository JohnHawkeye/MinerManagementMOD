using Terraria;
using Terraria.ID;

namespace MinerManagementMOD.Items.Treasure
{
    public class Treasure060 : TreasureBase
    {
        public override int TreasureID => 60;

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.value = Item.buyPrice(silver: Main.rand.Next(1, 20));
            Item.rare = ItemRarityID.Blue;        }
    }
}
