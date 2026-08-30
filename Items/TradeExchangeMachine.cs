using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MinerManagementMOD.Items
{
    public class TradeExchangeMachine : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.autoReuse = false;

            Item.useTurn = true;

            Item.maxStack = 99;

            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(silver: 50);

            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.TradeExchangeMachineTile>());
        }

    }
}