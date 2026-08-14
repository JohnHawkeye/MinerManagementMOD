using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using MinerManagementMOD.NPCs;
using MinerManagementMOD.Systems;

namespace MinerManagementMOD.Systems
{
    public static class MinerManager
    {

        public static bool IsMinerSpawned(int id)
        {
            foreach (NPC npc in Main.npc)
            {
                if (npc.active &&
                    npc.ModNPC is MinerNPC miner &&
                    miner.MinerID == id)
                {
                    return true;
                }
            }

            return false;
        }

        public static void SpawnMiner(Player player, MinerData data)
        {
            
            // 既に存在する場合
            if(IsMinerSpawned(data.ID))
            {
                Main.NewText(
                    $"{data.Name} は既に召喚されています。",
                    255,200,100
                );

                return;
            }

            int npcType;

            if(data.ID == 3)
            {
                npcType = ModContent.NPCType<MagicMinerNPC>();
            }
            else
            {
                npcType = ModContent.NPCType<MinerNPC>();
            }

            int npcID = NPC.NewNPC(
                null,
                (int)player.Center.X,
                (int)player.Center.Y,
                npcType
            );

            NPC npc = Main.npc[npcID];

            if(npc.ModNPC is MinerNPC miner)
            {
                miner.MinerID = data.ID;
                miner.MinerName = data.Name;
                miner.MiningLevel = data.MiningLevel;
                miner.MiningPower = data.MiningPower;
                miner.MiningSpeed = data.MiningSpeed;
                miner.OreBonusChance = data.OreBonusChance;
                miner.CarryCapacity = data.CarryCapacity;
                miner.HasLight = data.HasLight;

                npc.GivenName = data.Name;
            }

            Main.NewText(
                $"{data.Name} を召喚しました。",
                100,255,100
            );
        }
    }
}