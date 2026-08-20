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
        public string StyleInfo;

        public bool HasLight = false;

        public int MiningLevel = 1;
        public int MiningPower = 1;
        public int MiningSpeed = 1;
        public int CarryCapacity = 100;
        public int OreBonusChance = 30;

        public MinerData(
            int id, string name, string texture, string styleInfo)
        {
            ID = id;
            Name = name;
            TexturePath = texture;
            StyleInfo = styleInfo;
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

        public static MinerData CreateMiner(string name, string texture, string styleInfo)
        {
            for (int i = 0; i < MaxMiner; i++)
            {
                if (Miners[i].ID == 0)
                {
                    MinerData data = new MinerData(
                        nextMinerID,
                        name,
                        texture,
                        styleInfo
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

            if (Miners == null)
            {
                tag["Miners"] = minerList;
                return;
            }


            foreach (var miner in Miners)
            {
                if (miner == null)
                    continue;

                minerList.Add(new TagCompound
                {
                    ["ID"] = miner.ID,
                    ["IsHired"] = miner.IsHired,
                    ["HasLight"] = miner.HasLight,
                    ["MiningLevel"] = miner.MiningLevel,
                    ["MiningPower"] = miner.MiningPower,
                    ["MiningSpeed"] = miner.MiningSpeed,
                    ["CarryCapacity"] = miner.CarryCapacity,
                    ["OreBonusChance"] = miner.OreBonusChance,
                    ["Name"] = miner.Name ?? "名無し",
                    ["StyleInfo"] = miner.StyleInfo ?? ""
                });
            }

            tag["Miners"] = minerList;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            if (Miners == null)
            {
                ResetAll();
            }

            if (!tag.ContainsKey("Miners"))
                return;

            var minerList = tag.GetList<TagCompound>("Miners");

            for (int i = 0; i < minerList.Count && i < MaxMiner; i++)
            {
                var data = minerList[i];

                if (data.ContainsKey("ID"))
                    Miners[i].ID = data.GetInt("ID");

                Miners[i].IsHired = data.GetBool("IsHired");
                Miners[i].HasLight = data.GetBool("HasLight");

                if (data.ContainsKey("MiningLevel"))
                    Miners[i].MiningLevel = data.GetInt("MiningLevel");
                else
                    Miners[i].MiningLevel = 1;

                Miners[i].MiningPower = data.GetInt("MiningPower");
                Miners[i].MiningSpeed = data.GetInt("MiningSpeed");
                Miners[i].CarryCapacity = data.GetInt("CarryCapacity");
                Miners[i].OreBonusChance = data.GetInt("OreBonusChance");

                if (data.ContainsKey("Name"))
                    Miners[i].Name = data.GetString("Name");

                if (data.ContainsKey("StyleInfo"))
                    Miners[i].StyleInfo = data.GetString("StyleInfo");
                else
                    Miners[i].StyleInfo = "";
            }
        }

        public static void ResetAll()
        {
            nextMinerID = 1;

            Miners = new MinerData[MaxMiner];

            // 1人目：通常鉱夫1
            Miners[0] = CreateDefaultMiner(
                nextMinerID++,
                "ビル",
                "MinerManagementMOD/Assets/UI/MinerPortrait",
                "主人の場所から直線的に採掘を始める。\n稀に鉱石ボーナスが得られる。",
                miningLevel: 1,
                miningPower: 1,
                miningSpeed: 1,
                carryCapacity: 100,
                oreBonusChance: 30,
                hasLight: false
            );

            Miners[0].IsHired = true;

            // 2人目：通常鉱夫2
            Miners[1] = CreateDefaultMiner(
                nextMinerID++,
                "ベン",
                "MinerManagementMOD/Assets/UI/MinerPortrait",
                "主人の場所から直線的に採掘を始める。\n稀に鉱石ボーナスが得られる。",
                miningLevel: 1,
                miningPower: 1,
                miningSpeed: 1,
                carryCapacity: 100,
                oreBonusChance: 30,
                hasLight: false
            );

            // 3人目：魔法採掘師
            Miners[2] = CreateDefaultMiner(
                nextMinerID++,
                "魔法採掘師",
                "MinerManagementMOD/Assets/UI/MagicMiner",
                "主人に付き添い、鉱石を探知したらその方向に魔法を飛ばして採掘する。\n稀に宝石ボーナスが得られる。",
                miningLevel: 1,
                miningPower: 1,
                miningSpeed: 1,
                carryCapacity: 80,
                oreBonusChance: 30,
                hasLight: false
            );

            // 4人目：通常鉱夫（仮）
            Miners[3] = CreateDefaultMiner(
                nextMinerID++,
                "通常鉱夫3",
                "MinerManagementMOD/Assets/UI/EmptyPortrait",
                "主人の場所から直線的に採掘を始める。\n稀に鉱石ボーナスが得られる。",
                miningLevel: 1,
                miningPower: 1,
                miningSpeed: 1,
                carryCapacity: 80,
                oreBonusChance: 30,
                hasLight: false
            );

            // 5人目：採掘の女神
            Miners[4] = CreateDefaultMiner(
                nextMinerID++,
                "採掘の女神",
                "MinerManagementMOD/Assets/UI/MiningGoddess",
                "主人の背後に浮かび、ゴールドマイナーコインを捧げることで、周囲30ブロック以内の鉱石を一瞬で採掘する。",
                miningLevel: 1,
                miningPower: 999,
                miningSpeed: 999,
                carryCapacity: 9999,
                oreBonusChance: 0,
                hasLight: true
            );
        }

        public static MinerData CreateDefaultMiner(
            int id,
            string name,
            string texture,
            string styleInfo,
            int miningLevel = 1,
            int miningPower = 1,
            int miningSpeed = 1,
            int carryCapacity = 100,
            int oreBonusChance = 30,
            bool hasLight = false)
        {
            MinerData miner = new MinerData(id, name, texture, styleInfo);

            miner.IsHired = false;
            miner.MiningLevel = miningLevel;
            miner.MiningPower = miningPower;
            miner.MiningSpeed = miningSpeed;
            miner.CarryCapacity = carryCapacity;
            miner.OreBonusChance = oreBonusChance;
            miner.HasLight = hasLight;

            return miner;
        }
    }
}