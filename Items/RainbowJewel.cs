using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MinerManagementMOD.Items
{
    public class RainbowJewel : ModItem
    {
        public override void SetDefaults()
        {
            // アイテムサイズ
            Item.width = 32;
            Item.height = 32;

            // 最大スタック
            Item.maxStack = 9999;

            // 希少度
            Item.rare = ItemRarityID.Pink;

            // 売却価格
            // すべての宝石の平均価格より少し高め
            Item.value = Item.sellPrice(
                gold: 1,
                silver: 20
            );

            // マウスで使用できるアイテム
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useAnimation = 20;
            Item.useTime = 20;

            // 現時点では通常使用しない
            Item.consumable = false;
        }


        // ============================================================
        // クラフト
        // ============================================================

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();

            // 宝石を1個ずつ
            recipe.AddIngredient(ItemID.Amethyst, 1);
            recipe.AddIngredient(ItemID.Topaz, 1);
            recipe.AddIngredient(ItemID.Sapphire, 1);
            recipe.AddIngredient(ItemID.Emerald, 1);
            recipe.AddIngredient(ItemID.Ruby, 1);
            recipe.AddIngredient(ItemID.Diamond, 1);
            recipe.AddIngredient(ItemID.Amber,1);

            // クラフト作業台
            recipe.AddTile(TileID.WorkBenches);

            recipe.Register();
        }
    }
}