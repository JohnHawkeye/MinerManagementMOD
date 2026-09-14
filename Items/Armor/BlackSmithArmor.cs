using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace MinerManagementMOD.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class BlackSmithArmor : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 18;

            Item.value = Item.buyPrice(gold: 110);
            Item.rare = ItemRarityID.Yellow;

            Item.defense = 20;
        }

        public override void UpdateEquip(Player player)
        {
            // 全攻撃力 +6%
            player.GetDamage(DamageClass.Generic) += 0.06f;
        }

        public override void AddRecipes()
        {
            // チタンバー または アダマンタイトバー ×20
            // ゴールドマイナーコイン ×100
            CreateRecipe()
                .AddRecipeGroup("TitaniumOrAdamantiteBar", 20)
                .AddIngredient<GoldMinerCoin>(100)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}