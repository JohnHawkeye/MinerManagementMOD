using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using MinerManagementMOD.Systems;
using Terraria.DataStructures;

namespace MinerManagementMOD.Items
{
    public class ReturnEmblem : ModItem
    {
        // ==========================================
        // 設定
        // ==========================================

        // 帰還可能な最大距離（ブロック）
        private const float MaxReturnDistance = 3000f;


        // ==========================================
        // アイテム基本設定
        // ==========================================

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;

            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTime = 30;
            Item.useAnimation = 30;

            Item.UseSound = SoundID.Item6;

            Item.maxStack = 1;
            Item.rare = ItemRarityID.Blue;

            Item.value = Item.buyPrice(
                silver: 50
            );

            Item.consumable = false;
        }


        // ==========================================
        // 使用時
        // ==========================================

        public override bool? UseItem(Player player)
        {
            // ==========================================
            // アウトポストが存在しない
            // ==========================================

            if (MinerStationSystem.OutpostPositions.Count == 0)
            {
                Main.NewText(
                    "帰還できるアウトポストがありません。",
                    Color.Orange
                );

                return false;
            }


            // ==========================================
            // プレイヤーの現在位置
            // ==========================================

            Vector2 playerPosition =
                player.Center;


            // ==========================================
            // 最寄りのアウトポストを検索
            // ==========================================

            Point16 nearestOutpost =
                new Point16(-1, -1);

            float nearestDistanceSquared =
                float.MaxValue;


            foreach (Point16 position in
                     MinerStationSystem.OutpostPositions)
            {
                // ------------------------------------------
                // Outpost中央のワールド座標
                // ------------------------------------------

                Vector2 outpostPosition =
                    new Vector2(
                        position.X * 16f + 16f,
                        position.Y * 16f + 32f
                    );


                // ------------------------------------------
                // 距離の二乗を計算
                // ------------------------------------------

                float distanceSquared =
                    Vector2.DistanceSquared(
                        playerPosition,
                        outpostPosition
                    );


                // ------------------------------------------
                // より近ければ更新
                // ------------------------------------------

                if (distanceSquared <
                    nearestDistanceSquared)
                {
                    nearestDistanceSquared =
                        distanceSquared;

                    nearestOutpost =
                        position;
                }
            }


            // ==========================================
            // 最寄りのアウトポストが見つからない
            // ==========================================

            if (nearestOutpost.X < 0 ||
                nearestOutpost.Y < 0)
            {
                return false;
            }


            // ==========================================
            // 3000ブロック以内か確認
            // ==========================================

            float maxDistancePixels =
                MaxReturnDistance * 16f;

            float maxDistanceSquared =
                maxDistancePixels *
                maxDistancePixels;


            if (nearestDistanceSquared >
                maxDistanceSquared)
            {
                Main.NewText(
                    "近くに帰還できるアウトポストがありません。",
                    Color.Orange
                );

                return false;
            }


            // ==========================================
            // Outpost中央へテレポート
            // ==========================================

            Vector2 teleportPosition =
                new Vector2(
                    nearestOutpost.X * 16f + 16f,
                    nearestOutpost.Y * 16f + 32f
                );


            player.Teleport(
                teleportPosition,
                0
            );


            // ==========================================
            // 成功メッセージ
            // ==========================================

            Main.NewText(
                "アウトポストへ帰還しました。",
                Color.LightGreen
            );


            return true;
        }


        // ==========================================
        // レシピ
        // ==========================================

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.MagicMirror)
                .AddIngredient(ItemID.Silk, 5)
                .AddIngredient<GoldMinerCoin>(10)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}