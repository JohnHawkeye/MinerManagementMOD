using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace MinerManagementMOD.Systems
{
    public class TreasureWorldData : ModSystem
    {
        // 開封済みの宝箱
        public static HashSet<int> OpenedTreasureChests = new();

        public override void OnWorldLoad()
        {
            OpenedTreasureChests.Clear();
        }

        public override void OnWorldUnload()
        {
            OpenedTreasureChests.Clear();
        }

        public override void SaveWorldData(TagCompound tag)
        {
            tag["OpenedTreasureChests"] = new List<int>(OpenedTreasureChests);
        }

        public override void LoadWorldData(TagCompound tag)
        {
            OpenedTreasureChests.Clear();

            if (tag.ContainsKey("OpenedTreasureChests"))
            {
                foreach (int index in tag.GetList<int>("OpenedTreasureChests"))
                    OpenedTreasureChests.Add(index);
            }
        }
    }
}