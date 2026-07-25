using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace MinerManagementMOD.Systems
{
    public class TreasureWorldSystem : ModSystem
    {
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            // 「マイクロバイオーム生成」の後に実行
            int index = tasks.FindIndex(pass => pass.Name.Equals("Micro Biomes"));

            if (index != -1)
            {
                tasks.Insert(index + 1, new PassLegacy("MinerManagement Treasure", AddTreasures));
            }
            else
            {
                // 見つからなければ最後に追加
                tasks.Add(new PassLegacy("MinerManagement Treasure", AddTreasures));
            }
        }

        private void AddTreasures(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "作者の気持ちを詰め込んでいます...";

            List<int> treasureIDs = new List<int>();
            for (int i = 1; i <= 128; i++)
            {
                treasureIDs.Add(i);
            }

            // Fisher-Yatesシャッフル
            for (int i = treasureIDs.Count - 1; i > 0; i--)
            {
                int j = WorldGen.genRand.Next(i + 1);

                int temp = treasureIDs[i];
                treasureIDs[i] = treasureIDs[j];
                treasureIDs[j] = temp;
            }

            int treasureIndex = 0;

            foreach (Chest chest in Main.chest)
            {
                if (chest == null)
                    continue;

                if (treasureIndex >= treasureIDs.Count)
                    break;

                // 空きスロットを探す
                for (int i = 0; i < Chest.maxItems; i++)
                {
                    if (!chest.item[i].IsAir)
                        continue;


                    int id = treasureIDs[treasureIndex++];
                    string itemName = $"Treasure{id:000}";

                    if (Mod.TryFind(itemName, out ModItem modItem))
                    {
                        chest.item[i].SetDefaults(modItem.Type);
                        chest.item[i].stack = 1;
                    }

                    break;
                }
            }
        }
    }
}