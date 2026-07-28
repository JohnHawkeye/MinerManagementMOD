using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MinerManagementMOD.Items
{
    public class CrystalVacuum : ModItem
    {
        // 吸引(採掘)の有効半径(タイル数)
        private const int VacuumRadiusTiles = 6;

        // クリックとして認識する最大距離(プレイヤーからのタイル数)
        private const int MaxClickRangeTiles = 16;

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 28;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTurn = true;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.maxStack = 1;
            Item.value = Item.sellPrice(gold: 2);
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item4;
        }

        public override bool? UseItem(Player player)
        {
            // タイル操作・判定はローカルプレイヤーのみが行う(サーバーには結果を同期する)
            if (player.whoAmI != Main.myPlayer)
                return true;

            Vector2 mouseWorld = Main.MouseWorld;
            Vector2 playerCenter = player.Center;
            Vector2 offset = mouseWorld - playerCenter;

            // クリック位置がプレイヤーから MaxClickRangeTiles を超えていたら、その方向の限界地点にクランプする
            float maxRangePixels = MaxClickRangeTiles * 16f;
            if (offset.Length() > maxRangePixels)
            {
                offset = Vector2.Normalize(offset) * maxRangePixels;
            }

            Vector2 targetWorld = playerCenter + offset;
            int centerTileX = (int)(targetWorld.X / 16f);
            int centerTileY = (int)(targetWorld.Y / 16f);

            HarvestCrystalsInRadius(player, centerTileX, centerTileY, VacuumRadiusTiles);
                SoundEngine.PlaySound(
                    SoundID.Item23, // ドリル系の駆動音(Drax/Pickaxe Axeなどで使用)
                    player.Center
                );
            return true;
        }

        private void HarvestCrystalsInRadius(Player player, int centerX, int centerY, int radius)
        {
            for (int x = centerX - radius; x <= centerX + radius; x++)
            {
                for (int y = centerY - radius; y <= centerY + radius; y++)
                {
                    if (!WorldGen.InWorld(x, y, 1))
                        continue;

                    // 円形範囲に絞り込む(正方形ではなく半径6タイルの円)
                    float dist = Vector2.Distance(new Vector2(x, y), new Vector2(centerX, centerY));
                    if (dist > radius)
                        continue;

                    Tile tile = Main.tile[x, y];
                    if (tile == null || !tile.HasTile)
                        continue;

                    // Crystal Shard タイル以外には一切干渉しない
                    if (tile.TileType != TileID.Crystals)
                        continue;

                    MineAndPullCrystal(player, x, y);
                }
            }
        }

        private void MineAndPullCrystal(Player player, int x, int y)
        {
            // 通常のアイテムドロップはさせず、タイルのみ破壊する(noItem: true)
            WorldGen.KillTile(x, y, fail: false, effectOnly: false, noItem: true);

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                // 他クライアント/サーバーへタイル破壊を同期
                NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, x, y);
            }

            Vector2 itemSpawnPos = new Vector2(x * 16, y * 16);

            int itemIndex = Item.NewItem(
                player.GetSource_ItemUse(Item),
                itemSpawnPos,
                Type: ItemID.CrystalShard,
                Stack: 1
            );

            Item spawnedItem = Main.item[itemIndex];
            spawnedItem.noGrabDelay = 0;
            spawnedItem.playerIndexTheItemIsReservedFor = player.whoAmI; // 他プレイヤーに横取りされないようにする

            var vacuumData = spawnedItem.GetGlobalItem<CrystalVacuumGlobalItem>();
            vacuumData.StartVacuum(player.whoAmI);

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendData(MessageID.SyncItem, -1, -1, null, itemIndex, 1f);
            }
        }

         public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.CrystalShard, 50)
                .AddRecipeGroup("CobaltOrPalladium", 10)
                .AddIngredient(ItemID.Wire, 10)
                .AddIngredient(ItemID.TreasureMagnet, 1)
                .AddIngredient(ItemID.GoldCoin, 50)
                .AddTile(TileID.Anvils)
                .Register();

        }
        
    }
}