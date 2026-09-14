using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace MinerManagementMOD.Items.Armor
{
    [AutoloadEquip(EquipType.Legs)]
    public class BlackSmithLeggings : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 18;

            Item.value = Item.buyPrice(gold: 42);
            Item.rare = ItemRarityID.Yellow;

            Item.defense = 10;
        }

        public override void UpdateEquip(Player player)
        {
            // 移動速度 +6%
            player.moveSpeed += 0.06f;
        }

        public override void AddRecipes()
        {
            // チタンバー または アダマンタイトバー ×14
            // ゴールドマイナーコイン ×40
            CreateRecipe()
                .AddRecipeGroup("TitaniumOrAdamantiteBar", 14)
                .AddIngredient<GoldMinerCoin>(40)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}