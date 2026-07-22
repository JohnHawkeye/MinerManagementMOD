using Terraria;
using Terraria.ID;

namespace MinerManagementMOD.Items.Treasure
{
    public class Treasure071 : TreasureBase
    {
        public override int TreasureID => 71;

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.value = Item.buyPrice(silver: Main.rand.Next(20, 40));
            Item.rare = ItemRarityID.Blue;
        }
    }
}
