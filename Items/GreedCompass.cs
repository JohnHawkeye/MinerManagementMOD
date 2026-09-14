using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using MinerManagementMOD.Buffs;
using MinerManagementMOD.Common.Players;

namespace MinerManagementMOD.Items
{
    public class GreedCompass : ModItem
    {
        // 宝箱を検索する最大距離
        // 3000px = 187.5タイル
        public const float SearchRange = 3000f;

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;

            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.UseSound = SoundID.Item4;

            Item.rare = ItemRarityID.Yellow;
            Item.value = Item.buyPrice(gold: 5);

            Item.noMelee = true;
            Item.autoReuse = false;
            Item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return true;
        }

        public override bool? UseItem(Player player)
        {
            // ローカルプレイヤーだけが検索データを保持する
            if (Main.myPlayer == player.whoAmI)
            {
                GreedCompassPlayer compassPlayer =
                    player.GetModPlayer<GreedCompassPlayer>();

                bool found = compassPlayer.ScanForChests(SearchRange);

                if (found)
                {
                    player.AddBuff(
                        ModContent.BuffType<DetectChestBuff>(),
                        60
                    );
                }
                else
                {
                    Main.NewText(
                        "近くに宝箱は見つからなかった……。",
                        Color.LightGray
                    );
                }
            }

            return true;
        }

        public override void AddRecipes()
        {
            // レシピは後で決める
            //
            // 例：
            // CreateRecipe()
            //     .AddIngredient(ItemID.GoldWatch)
            //     .AddIngredient(ItemID.GoldBar, 5)
            //     .AddTile(TileID.TinkerersWorkbench)
            //     .Register();
        }
    }
}