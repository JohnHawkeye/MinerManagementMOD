using System.Data;
using Terraria.ModLoader;

namespace MinerManagementMOD.Systems
{

    public class MinerData
    {
        public int ID;
        public bool IsHired = false;

        public string Name;
        public string TexturePath;

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
            Miners = new MinerData[MaxMiner];

            Miners[0] = new MinerData(nextMinerID, "マイナー", "MinerManagementMOD/Assets/UI/MinerPortrait");
            Miners[0].IsHired = true;

            nextMinerID++;
            
            for (int i = 1; i < MaxMiner; i++)
            {
                Miners[i] = new MinerData(
                    nextMinerID, "空き", "MinerManagementMOD/Assets/UI/EmptyPortrait");
                
                Miners[i].IsHired = false;
            }
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
    }
}