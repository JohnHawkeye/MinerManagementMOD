using Terraria;
using Terraria.ID;

namespace MinerManagementMOD.Items.Treasure
{
    public class Treasure070 : TreasureBase
    {
        public override int TreasureID => 70;

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.value = Item.buyPrice(silver: Main.rand.Next(10, 30));
            Item.rare = ItemRarityID.Blue;
        }
    }
}
