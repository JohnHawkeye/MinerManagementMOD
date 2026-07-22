using Terraria;
using Terraria.ID;

namespace MinerManagementMOD.Items.Treasure
{
    public class Treasure126 : TreasureBase
    {
        public override int TreasureID => 126;

        public override void SetDefaults()
        {
            base.SetDefaults();


            Item.value = Item.buyPrice(gold: Main.rand.Next(80, 100));
            Item.rare = ItemRarityID.LightRed;
        }
    }
}
