using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace MinerManagementMOD.Items.Armor
{
    [AutoloadEquip(EquipType.Legs)]
    public class ReinforcedMiningPants : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 18;

            Item.value = Item.buyPrice(gold: 5);
            Item.rare = ItemRarityID.Yellow;

            Item.defense = 5;
        }

        public override void UpdateEquip(Player player)
        {
            // 移動速度 +6%
            player.moveSpeed += 0.06f;

            // 採掘速度 +10%
            player.pickSpeed -= 0.10f;
        }

        public override void AddRecipes()
        {

            CreateRecipe()
                .AddRecipeGroup("GoldOrPlatinumBar", 20)
                .AddIngredient(ItemID.MiningPants, 1)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}