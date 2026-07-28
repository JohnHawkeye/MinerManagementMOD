using System.Collections.Generic;
using System.Data;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace MinerManagementMOD.Systems
{
    public class MinerData
    {
        public int ID;
        public bool IsHired = false;

        public string Name;
        public string TexturePath;

        public bool HasLight = false;

        public int MiningPower = 1;
        public int MiningSpeed = 1;
        public int CarryCapacity = 100;

        public MinerData(int id, string name, string texture)
        {
            ID = id;
            Name = name;
            TexturePath = texture;

        }
    }

    public class MinerRosterSystem : ModSystem
    {
        private static int nextMinerID = 1;
        public const int MaxMiner = 5;

        public static MinerData[] Miners;

        //
        public static int GetNextID()
        {
            return nextMinerID++;
        }

        public override void OnWorldLoad()
        {
            ResetAll();
        }

        public static MinerData CreateMiner(string name, string texture)
        {
            for (int i = 0; i < MaxMiner; i++)
            {
                if (Miners[i].ID == 0)
                {
                    MinerData data = new MinerData(
                        nextMinerID,
                        name,
                        texture
                    );

                    nextMinerID++;

                    Miners[i] = data;

                    return data;
                }
            }
            return null;
        }

        public override void SaveWorldData(TagCompound tag)
        {
            var minerList = new List<TagCompound>();

            foreach (var miner in Miners)
            {
                minerList.Add(new TagCompound
                {
                    ["IsHired"] = miner.IsHired,
                    ["HasLight"] = miner.HasLight,
                    ["MiningPower"] = miner.MiningPower,
                    ["MiningSpeed"] = miner.MiningSpeed,
                    ["CarryCapacity"] = miner.CarryCapacity,
                    ["Name"] = miner.Name
                });
            }

            tag["Miners"] = minerList;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            if (!tag.ContainsKey("Miners"))
                return;

            var minerList = tag.GetList<TagCompound>("Miners");

            for (int i = 0; i < minerList.Count && i < MaxMiner; i++)
            {
                var data = minerList[i];

                Miners[i].IsHired = data.GetBool("IsHired");
                Miners[i].HasLight = data.GetBool("HasLight");

                Miners[i].MiningPower = data.GetInt("MiningPower");
                Miners[i].MiningSpeed = data.GetInt("MiningSpeed");
                Miners[i].CarryCapacity = data.GetInt("CarryCapacity");

                Miners[i].Name = data.GetString("Name");
            }
        }

        public static void ResetAll()
        {
            nextMinerID = 1;

            Miners = new MinerData[MaxMiner];

            Miners[0] = new MinerData(
                nextMinerID,
                "マイナー",
                "MinerManagementMOD/Assets/UI/MinerPortrait");

            Miners[0].IsHired = true;
            nextMinerID++;

            for (int i = 1; i < MaxMiner; i++)
            {
                Miners[i] = new MinerData(
                    nextMinerID,
                    "空き",
                    "MinerManagementMOD/Assets/UI/EmptyPortrait");

                Miners[i].IsHired = false;
                nextMinerID++;
            }
        }
    }
}