using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace MinerManagementMOD.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class BlackSmithHat : ModItem
    {
        public override void SetStaticDefaults()
        {
            ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 20;

            Item.value = Item.buyPrice(gold: 65);
            Item.rare = ItemRarityID.Yellow;

            Item.defense = 20;
            
        }

        public override void UpdateEquip(Player player)
        {
            // 全クリティカル率 +8%
            player.GetCritChance(DamageClass.Generic) += 8;
        }

        public override void AddRecipes()
        {
            // チタンバー または アダマンタイトバー ×16
            // ゴールドマイナーコイン ×60
            CreateRecipe()
                .AddRecipeGroup("TitaniumOrAdamantiteBar", 16)
                .AddIngredient<GoldMinerCoin>(60)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}