using Terraria;
using Terraria.ID;

namespace MinerManagementMOD.Items.Treasure
{
    public class Treasure128 : TreasureBase
    {
        public override int TreasureID => 128;

        public override void SetDefaults()
        {
            base.SetDefaults();


            Item.value = Item.buyPrice(platinum: 10);
            Item.rare = ItemRarityID.Pink;
        }
    }
}
