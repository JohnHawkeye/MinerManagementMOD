using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using MinerManagementMOD.Systems;

namespace MinerManagementMOD.Items
{
    public class DungeonMap : ModItem
    {
        public override void SetDefaults()
        {
            // ========================================================
            // 基本設定
            // ========================================================

            Item.width = 32;
            Item.height = 32;

            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTime = 30;
            Item.useAnimation = 30;

            Item.UseSound = SoundID.Item4;

            Item.rare = ItemRarityID.Orange;

            Item.consumable = true;

            // スタック可能
            Item.maxStack = 1;

            // 使用時にアイテムを置かない
            Item.noMelee = true;
            
            Item.value = Item.buyPrice(
                gold: 50
            );
        }

        // ============================================================
        // 使用可能判定
        // ============================================================

        public override bool CanUseItem(Player player)
        {
            // すでに解析中なら使用不可
            if (DungeonMapSystem.IsRevealing)
                return false;

            return true;
        }

        // ============================================================
        // 使用
        // ============================================================

        public override bool? UseItem(Player player)
        {
            DungeonMapSystem.StartReveal(player);

            return true;
        }

    }
}