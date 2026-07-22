using Terraria;
using Terraria.ID;

namespace MinerManagementMOD.Items.Treasure
{
    public class Treasure108 : TreasureBase
    {
        public override int TreasureID => 108;

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.value = Item.buyPrice(gold: Main.rand.Next(10, 20));
            Item.rare = ItemRarityID.Orange;
        }
    }
}
