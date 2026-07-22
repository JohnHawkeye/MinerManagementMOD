using Terraria;
using Terraria.ID;

namespace MinerManagementMOD.Items.Treasure
{
    public class Treasure125 : TreasureBase
    {
        public override int TreasureID => 125;

        public override void SetDefaults()
        {
            base.SetDefaults();


            Item.value = Item.buyPrice(gold: Main.rand.Next(80, 90));
            Item.rare = ItemRarityID.LightRed;
        }
    }
}
