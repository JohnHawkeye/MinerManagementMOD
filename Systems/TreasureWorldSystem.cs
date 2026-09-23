using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.WorldBuilding;

namespace MinerManagementMOD.Systems
{
    public class TreasureWorldSystem : ModSystem
    {
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            int index = tasks.FindIndex(pass => pass.Name.Equals("Micro Biomes"));

            if (index != -1)
            {
                tasks.Insert(index + 1, new PassLegacy(
                    "MinerManagement Treasure",
                    AddTreasures));
            }
            else
            {
                tasks.Add(new PassLegacy(
                    "MinerManagement Treasure",
                    AddTreasures));
            }
        }

        private void AddTreasures(
            GenerationProgress progress,
            GameConfiguration configuration)
        {
            progress.Message = "鉱夫の気持ちを詰め込んでいます...";

            List<int> treasureIDs = new();

            for (int i = 1; i <= 128; i++)
                treasureIDs.Add(i);

            // ------------------------
            // トレジャーIDをシャッフル
            // ------------------------
            for (int i = treasureIDs.Count - 1; i > 0; i--)
            {
                int j = WorldGen.genRand.Next(i + 1);
                (treasureIDs[i], treasureIDs[j]) =
                    (treasureIDs[j], treasureIDs[i]);
            }

            int normalChestCount = CountGoldChests();

            int extraGoldChestCount =
                (int)(normalChestCount * 0.25f);

            for (int i = 0; i < extraGoldChestCount; i++)
            {
                PlaceGoldChest(false);
            }

            // ------------------------
            // すべてのチェストに宝を追加
            // ------------------------
            FillAllChests(treasureIDs);

            // ------------------------
            // 偽物のゴールドチェスト
            // ------------------------
            int fakeChestCount =
                (int)(normalChestCount *
                Main.rand.NextFloat(0.15f, 0.20f));

            for (int i = 0; i < fakeChestCount; i++)
            {
                PlaceGoldChest(true);
            }

            // ------------------------
            // ゴールドチェストを30%の確率でロック
            // ------------------------
            LockRandomGoldChests(0.30f);
        }

        private static int FindEmptySlot(Chest chest)
        {
            for (int i = 0; i < Chest.maxItems; i++)
            {
                if (chest.item[i].IsAir)
                    return i;
            }

            return -1;
        }

        private bool PlaceGoldChest(bool fakeChest)
        {
            for (int attempt = 0; attempt < 500; attempt++)
            {
                int x = WorldGen.genRand.Next(
                    100,
                    Main.maxTilesX - 100);

                int y = WorldGen.genRand.Next(
                    (int)Main.worldSurface,
                    (int)(Main.maxTilesY * 0.55f));

                if (IsDungeonArea(x, y))
                    continue;

                if (x < 10 || x > Main.maxTilesX - 10)
                    continue;

                if (y < 10 || y > Main.maxTilesY - 10)
                    continue;

                WorldGen.KillTile(x, y);
                WorldGen.KillTile(x + 1, y);

                int chestIndex = WorldGen.PlaceChest(
                    x,
                    y,
                    (ushort)TileID.Containers,
                    false,
                    1); // Gold Chest

                if (chestIndex >= 0)
                {
                    WorldGen.PlaceTile(
                        x,
                        y + 1,
                        TileID.Stone);

                    WorldGen.PlaceTile(
                        x + 1,
                        y + 1,
                        TileID.Stone);

                    if (fakeChest)
                    {
                        Chest chest = Main.chest[chestIndex];
                        AddFoolTicket(chest);
                    }

                    return true;
                }
            }

            return false;
        }

        private void FillAllChests(List<int> treasureIDs)
        {
            int treasureIndex = 0;

            foreach (Chest chest in Main.chest)
            {
                if (chest == null)
                    continue;

                // ==================================================
                // トレジャー
                // ==================================================
                if (treasureIndex < treasureIDs.Count)
                {
                    int slot = FindEmptySlot(chest);

                    if (slot != -1)
                    {
                        int id = treasureIDs[treasureIndex++];
                        string itemName = $"Treasure{id:000}";

                        if (Mod.TryFind(itemName, out ModItem modItem))
                        {
                            chest.item[slot].SetDefaults(modItem.Type);
                            chest.item[slot].stack = 1;
                        }
                    }
                }

                // ==================================================
                // インゴット×2
                //
                // 鉄       40%
                // 銅       25%
                // 銀       25%
                // 金       10%
                // ==================================================
                for (int i = 0; i < 2; i++)
                {
                    int slot = FindEmptySlot(chest);

                    if (slot == -1)
                        break;

                    int bar = WorldGen.genRand.Next(100);

                    int barType;

                    if (bar < 40)
                    {
                        // 40%
                        barType = ItemID.IronBar;
                    }
                    else if (bar < 65)
                    {
                        // 25%
                        barType = ItemID.CopperBar;
                    }
                    else if (bar < 90)
                    {
                        // 25%
                        barType = ItemID.SilverBar;
                    }
                    else
                    {
                        // 10%
                        barType = ItemID.GoldBar;
                    }

                    chest.item[slot].SetDefaults(barType);

                    // 金だけ少なめ
                    if (barType == ItemID.GoldBar)
                    {
                        chest.item[slot].stack =
                            WorldGen.genRand.Next(3, 7); // 3～6
                    }
                    else
                    {
                        chest.item[slot].stack =
                            WorldGen.genRand.Next(6, 13); // 6～12
                    }
                }

                // ==================================================
                // コイン×2
                //
                // 銅貨 50%
                // 銀貨 35%
                // 金貨 15%
                // ==================================================
                for (int i = 0; i < 2; i++)
                {
                    int slot = FindEmptySlot(chest);

                    if (slot == -1)
                        break;

                    int coin = WorldGen.genRand.Next(100);

                    int coinType;

                    if (coin < 50)
                    {
                        // 50%
                        coinType = ItemID.CopperCoin;
                    }
                    else if (coin < 85)
                    {
                        // 35%
                        coinType = ItemID.SilverCoin;
                    }
                    else
                    {
                        // 15%
                        coinType = ItemID.GoldCoin;
                    }

                    chest.item[slot].SetDefaults(coinType);

                    // 金貨だけ少なめ
                    if (coinType == ItemID.GoldCoin)
                    {
                        chest.item[slot].stack =
                            WorldGen.genRand.Next(5, 31); // 5～30
                    }
                    else if (coinType == ItemID.SilverCoin)
                    {
                        chest.item[slot].stack =
                            WorldGen.genRand.Next(20, 61); // 20～60
                    }
                    else
                    {
                        chest.item[slot].stack =
                            WorldGen.genRand.Next(30, 101); // 30～100
                    }
                }

                // ==================================================
                // マイナーコイン×1
                //
                // Copper Miner Coin 55%
                // Silver Miner Coin 35%
                // Gold Miner Coin   10%
                // ==================================================
                {
                    int slot = FindEmptySlot(chest);

                    if (slot != -1)
                    {
                        int coin = WorldGen.genRand.Next(100);

                        int coinType;

                        if (coin < 55)
                        {
                            // 55%
                            coinType =
                                ModContent.ItemType<
                                    Items.CopperMinerCoin>();
                        }
                        else if (coin < 90)
                        {
                            // 35%
                            coinType =
                                ModContent.ItemType<
                                    Items.SilverMinerCoin>();
                        }
                        else
                        {
                            // 10%
                            coinType =
                                ModContent.ItemType<
                                    Items.GoldMinerCoin>();
                        }

                        chest.item[slot].SetDefaults(coinType);

                        // Gold Miner Coinだけ少なめ
                        if (coinType ==
                            ModContent.ItemType<
                                Items.GoldMinerCoin>())
                        {
                            chest.item[slot].stack =
                                WorldGen.genRand.Next(3, 6); // 3～5
                        }
                        else if (coinType ==
                            ModContent.ItemType<
                                Items.SilverMinerCoin>())
                        {
                            chest.item[slot].stack =
                                WorldGen.genRand.Next(5, 11); // 5～10
                        }
                        else
                        {
                            chest.item[slot].stack =
                                WorldGen.genRand.Next(8, 16); // 8～15
                        }
                    }
                }

                // ==================================================
                // 宝石×1
                // ==================================================
                {
                    int slot = FindEmptySlot(chest);

                    if (slot != -1)
                    {
                        int[] gems =
                        {
                            ItemID.Amethyst,
                            ItemID.Topaz,
                            ItemID.Sapphire,
                            ItemID.Emerald,
                            ItemID.Ruby,
                            ItemID.Diamond,
                            ItemID.Amber
                        };

                        chest.item[slot].SetDefaults(
                            gems[WorldGen.genRand.Next(gems.Length)]);

                        chest.item[slot].stack =
                            WorldGen.genRand.Next(8, 13);
                    }
                }
            }
        }

        private bool IsDungeonArea(int x, int y)
        {
            // 周囲20マスを確認
            int range = 20;

            for (int xx = x - range; xx <= x + range; xx++)
            {
                for (int yy = y - range; yy <= y + range; yy++)
                {
                    if (!WorldGen.InWorld(xx, yy))
                        continue;

                    Tile tile = Main.tile[xx, yy];

                    if (!tile.HasTile)
                        continue;

                    if (tile.TileType == TileID.BlueDungeonBrick ||
                        tile.TileType == TileID.GreenDungeonBrick ||
                        tile.TileType == TileID.PinkDungeonBrick)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void AddFoolTicket(Chest chest)
        {
            for (int i = 0; i < Chest.maxItems; i++)
            {
                if (chest.item[i].IsAir)
                {
                    chest.item[i].SetDefaults(
                        ModContent.ItemType<Items.FoolTicket>()
                    );

                    chest.item[i].stack = 1;

                    return;
                }
            }
        }

        private int CountExistingChests()
        {
            int count = 0;

            foreach (Chest chest in Main.chest)
            {
                if (chest != null)
                {
                    count++;
                }
            }

            return count;
        }

        private int CountGoldChests()
        {
            int count = 0;

            foreach (Chest chest in Main.chest)
            {
                if (chest == null)
                    continue;

                Tile tile = Framing.GetTileSafely(
                    chest.x,
                    chest.y);

                if (!tile.HasTile ||
                    tile.TileType != TileID.Containers)
                    continue;

                if (TileObjectData.GetTileStyle(tile) == 1)
                    count++;
            }

            return count;
        }

        private static void LockRandomGoldChests(
            float lockChance = 0.3f)
        {
            for (int i = 0; i < Main.maxChests; i++)
            {
                Chest chest = Main.chest[i];

                if (chest == null)
                    continue;

                Tile tile = Framing.GetTileSafely(
                    chest.x,
                    chest.y);

                // コンテナ以外は無視
                if (!tile.HasTile ||
                    tile.TileType != TileID.Containers)
                    continue;

                // ゴールドチェスト以外は無視
                int style = TileObjectData.GetTileStyle(tile);

                if (style != 1)
                    continue;

                // 確率判定
                if (WorldGen.genRand.NextFloat() > lockChance)
                    continue;

                // ロック済みゴールドチェストへ変更
                LockChest(chest);
            }
        }

        private static void LockChest(Chest chest)
        {
            for (int x = chest.x; x < chest.x + 2; x++)
            {
                for (int y = chest.y; y < chest.y + 2; y++)
                {
                    Tile tile = Framing.GetTileSafely(x, y);

                    if (!tile.HasTile ||
                        tile.TileType != TileID.Containers)
                        continue;

                    tile.TileFrameX += 36;
                }
            }
        }
    }
}