using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace MinerManagementMOD.Items
{
    public class SlotMachine : ModItem
    {
        public override string Texture
            => "MinerManagementMOD/Assets/Tiles/SlotMachine";

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 64;

            Item.maxStack = 99;

            Item.useTurn = true;
            Item.autoReuse = true;

            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;

            Item.consumable = true;

            Item.createTile = ModContent.TileType<Tiles.SlotMachineTile>();

            Item.value = Item.buyPrice(silver: 50);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.IronBar, 10)
                .AddIngredient(ItemID.GoldBar, 5)
                .AddIngredient(ItemID.Wire, 50)
                .AddIngredient(ItemID.Glass, 10)
                .AddIngredient<PlatinumMinerCoin>(1)
                .Register();
        }
    }
}