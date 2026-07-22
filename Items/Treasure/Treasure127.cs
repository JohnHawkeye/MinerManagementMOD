using Terraria;
using Terraria.ID;

namespace MinerManagementMOD.Items.Treasure
{
    public class Treasure127 : TreasureBase
    {
        public override int TreasureID => 127;

        public override void SetDefaults()
        {
            base.SetDefaults();


            Item.value = Item.buyPrice(platinum: 10);
            Item.rare = ItemRarityID.Pink;
        }
    }
}
