using Terraria;
using Terraria.ID;

namespace MinerManagementMOD.Items.Treasure
{
    public class Treasure111 : TreasureBase
    {
        public override int TreasureID => 111;

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.value = Item.buyPrice(gold: Main.rand.Next(20, 30));
            Item.rare = ItemRarityID.Orange;
        }
    }
}
