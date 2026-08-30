using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using MinerManagementMOD.Systems;

namespace MinerManagementMOD.Items
{
    public class SkyIslandMap : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;

            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTime = 30;
            Item.useAnimation = 30;

            Item.UseSound = SoundID.Item4;

            Item.maxStack = 1;
            Item.consumable = true;

            Item.rare = ItemRarityID.Blue;

            Item.value = Item.buyPrice(
                gold: 50
            );

            Item.noMelee = true;
        }

        public override bool CanUseItem(Player player)
        {
            // マップ開放処理中は使用できない
            if (SkyMapSystem.IsRevealing)
                return false;

            return true;
        }

        public override bool? UseItem(Player player)
        {
            SkyMapSystem.StartReveal(player);

            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Feather, 5)
                .AddIngredient(ItemID.Cloud, 10)
                .AddIngredient(ItemID.GoldBar, 1)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}