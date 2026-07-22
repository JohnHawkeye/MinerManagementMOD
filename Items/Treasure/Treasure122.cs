using Terraria;
using Terraria.ID;

namespace MinerManagementMOD.Items.Treasure
{
    public class Treasure122 : TreasureBase
    {
        public override int TreasureID => 122;

        public override void SetDefaults()
        {
            base.SetDefaults();


            Item.value = Item.buyPrice(gold: Main.rand.Next(60, 70));
            Item.rare = ItemRarityID.LightRed;
        }
    }
}
