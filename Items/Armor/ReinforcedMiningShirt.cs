using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace MinerManagementMOD.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class ReinforcedMiningShirt : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 18;

            Item.value = Item.buyPrice(gold: 5);
            Item.rare = ItemRarityID.Yellow;

            Item.defense = 5;
        }

        public override void UpdateEquip(Player player)
        {
            // 採掘速度 +10%
            player.pickSpeed -= 0.10f;
        }

        // ==========================================
        // 鉱夫セット判定
        // ==========================================

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            // Mining Helmet + 強化版シャツ + 強化版パンツ
            if (head.type == ItemID.MiningHelmet &&
                legs.type == ModContent.ItemType<ReinforcedMiningPants>())
            {
                return true;
            }

            // Ultrabright Helmet + 強化版シャツ + 強化版パンツ
            if (head.type == ItemID.UltrabrightHelmet &&
                legs.type == ModContent.ItemType<ReinforcedMiningPants>())
            {
                return true;
            }

            return false;
        }

        public override void UpdateArmorSet(Player player)
        {
            // セットボーナス：採掘速度 +10%
            player.pickSpeed -= 0.10f;
        }
        
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup("GoldOrPlatinumBar", 20)
                .AddIngredient(ItemID.MiningShirt, 1)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}