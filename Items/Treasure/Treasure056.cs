using Terraria;
using Terraria.ID;

namespace MinerManagementMOD.Items.Treasure
{
    public class Treasure056 : TreasureBase
    {
        public override int TreasureID => 56;

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.value = Item.buyPrice(silver: Main.rand.Next(1, 20));
            Item.rare = ItemRarityID.Blue;        }
    }
}
