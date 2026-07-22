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

            foreach (Chest chest in Main.chest)
            {
                if (chest == null)
                    continue;

                 // 空きスロットを探す
                for (int i = 0; i < Chest.maxItems; i++)
                {
                    if (!chest.item[i].IsAir)
                        continue;

                    int id = WorldGen.genRand.Next(1, 49);

                    chest.item[i].SetDefaults(
                        ModContent.Find<ModItem>($"Treasure{id:000}").Type);

                    chest.item[i].stack = 1;
                    break;
                }
            }
        }
    }
}