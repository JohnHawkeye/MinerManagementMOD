using Terraria;
using Terraria.ID;

namespace MinerManagementMOD.Items.Treasure
{
    public class Treasure059 : TreasureBase
    {
        public override int TreasureID => 59;

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.value = Item.buyPrice(silver: Main.rand.Next(1, 20));
            Item.rare = ItemRarityID.Blue;        }
    }
}
