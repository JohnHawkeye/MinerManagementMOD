using Terraria;
using Terraria.ID;

namespace MinerManagementMOD.Items.Treasure
{
    public class Treasure055 : TreasureBase
    {
        public override int TreasureID => 55;

        public override void SetDefaults()
        {
            base.SetDefaults();

            Item.value = Item.buyPrice(silver: Main.rand.Next(1, 20));
            Item.rare = ItemRarityID.Blue;        }
    }
}
