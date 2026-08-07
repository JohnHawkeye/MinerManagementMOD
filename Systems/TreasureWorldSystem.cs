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
                tasks.Insert(index + 1, new PassLegacy("MinerManagement Treasure", AddTreasures));
            }
            else
            {
                tasks.Add(new PassLegacy("MinerManagement Treasure", AddTreasures));
            }
        }

        private void AddTreasures(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "鉱夫の気持ちを詰め込んでいます...";

            List<int> treasureIDs = new();

            for (int i = 1; i <= 128; i++)
                treasureIDs.Add(i);

            // シャッフル
            for (int i = treasureIDs.Count - 1; i > 0; i--)
            {
                int j = WorldGen.genRand.Next(i + 1);
                (treasureIDs[i], treasureIDs[j]) = (treasureIDs[j], treasureIDs[i]);
            }

            int normalChestCount = CountGoldChests();

            int extraGoldChestCount =
                (int)(normalChestCount * 0.25f);

            for (int i = 0; i < extraGoldChestCount; i++)
            {
                PlaceGoldChest(false);
            }

            FillAllChests(treasureIDs);

            int fakeChestCount =
                (int)(normalChestCount * Main.rand.NextFloat(0.15f, 0.20f));

            for (int i = 0; i < fakeChestCount; i++)
            {
                PlaceGoldChest(true);
            }

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

                // ------------------------
                // トレジャー
                // ------------------------
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

                // ------------------------
                // インゴット×3
                // ------------------------
                for (int i = 0; i < 3; i++)
                {
                    int slot = FindEmptySlot(chest);
                    if (slot == -1)
                        break;

                    int[] bars =
                    {
                        ItemID.CopperBar,
                        ItemID.SilverBar,
                        ItemID.GoldBar
                    };

                    chest.item[slot].SetDefaults(bars[WorldGen.genRand.Next(bars.Length)]);
                    chest.item[slot].stack = WorldGen.genRand.Next(8, 13); //8～12個
                }

                // ------------------------
                // コイン×3
                // ------------------------
                for (int i = 0; i < 3; i++)
                {
                    int slot = FindEmptySlot(chest);
                    if (slot == -1)
                        break;

                    int coin = WorldGen.genRand.Next(3);

                    switch (coin)
                    {
                        case 0:
                            chest.item[slot].SetDefaults(ItemID.CopperCoin);
                            break;

                        case 1:
                            chest.item[slot].SetDefaults(ItemID.SilverCoin);
                            break;

                        default:
                            chest.item[slot].SetDefaults(ItemID.GoldCoin);
                            break;
                    }

                    chest.item[slot].stack = WorldGen.genRand.Next(20, 81);
                }

                // ------------------------
                // マイナーコイン×1
                // ------------------------
                {
                    int slot = FindEmptySlot(chest);

                    if (slot != -1)
                    {
                        int coin = WorldGen.genRand.Next(3);

                        switch (coin)
                        {
                            case 0:
                                chest.item[slot].SetDefaults(ModContent.ItemType<Items.CopperMinerCoin>());
                                break;

                            case 1:
                                chest.item[slot].SetDefaults(ModContent.ItemType<Items.SilverMinerCoin>());
                                break;

                            default:
                                chest.item[slot].SetDefaults(ModContent.ItemType<Items.GoldMinerCoin>());
                                break;
                        }

                        chest.item[slot].stack = 10;
                    }
                }

                // ------------------------
                // 宝石×1
                // ------------------------
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

                        chest.item[slot].SetDefaults(gems[WorldGen.genRand.Next(gems.Length)]);
                        chest.item[slot].stack = WorldGen.genRand.Next(8, 13);
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

                Tile tile = Framing.GetTileSafely(chest.x, chest.y);

                if (!tile.HasTile || tile.TileType != TileID.Containers)
                    continue;

                if (TileObjectData.GetTileStyle(tile) == 1)
                    count++;
            }

            return count;
        }

        private static void LockRandomGoldChests(float lockChance = 0.3f)
        {
            for (int i = 0; i < Main.maxChests; i++)
            {
                Chest chest = Main.chest[i];
                if (chest == null)
                    continue;

                Tile tile = Framing.GetTileSafely(chest.x, chest.y);

                // コンテナ以外は無視
                if (!tile.HasTile || tile.TileType != TileID.Containers)
                    continue;

                // ゴールドチェスト以外は無視
                int style = TileObjectData.GetTileStyle(tile);
                if (style != 1) // Gold Chest
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

                    if (!tile.HasTile || tile.TileType != TileID.Containers)
                        continue;

                    tile.TileFrameX += 36;
                }
            }
        }
    }
}