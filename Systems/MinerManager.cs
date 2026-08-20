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
                if (!npc.active)
                    continue;

                if (npc.ModNPC is MinerNPC miner &&
                    miner.MinerID == id)
                {
                    return true;
                }
                if (npc.ModNPC is MagicMinerNPC magicMiner &&
                    magicMiner.MinerID == id)
                {
                    return true;
                }
                if (npc.ModNPC is MiningGoddessNPC miningGoddessNPC &&
                    miningGoddessNPC.MinerID == id)
                {
                    return true;
                }
            }

            return false;
        }

        public static void SpawnMiner(Player player, MinerData data)
        {
            // 既に存在する場合
            if (IsMinerSpawned(data.ID))
            {
                DespawnMiner(data.ID);

                Main.NewText(
                    $"{data.Name} の召喚を解除しました。",
                    255, 200, 100
                );

                return;
            }

            int npcType;

            if (data.ID == 5)
            {
                npcType = ModContent.NPCType<MiningGoddessNPC>();
            }
            else if (data.ID == 3)
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

            if (npc.ModNPC is MinerNPC miner)
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
            else if (npc.ModNPC is MagicMinerNPC magicMiner)
            {
                magicMiner.MinerID = data.ID;
                magicMiner.MinerName = data.Name;
                magicMiner.MiningLevel = data.MiningLevel;
                magicMiner.MiningPower = data.MiningPower;
                magicMiner.MiningSpeed = data.MiningSpeed;
                magicMiner.OreBonusChance = data.OreBonusChance;
                magicMiner.CarryCapacity = data.CarryCapacity;
                magicMiner.HasLight = data.HasLight;

                npc.GivenName = data.Name;
            }
            else if (npc.ModNPC is MiningGoddessNPC miningGoddessNPC)
            {
                miningGoddessNPC.MinerID = data.ID;
                miningGoddessNPC.MinerName = data.Name;
                miningGoddessNPC.MiningLevel = data.MiningLevel;
                miningGoddessNPC.MiningPower = data.MiningPower;
                miningGoddessNPC.MiningSpeed = data.MiningSpeed;
                miningGoddessNPC.OreBonusChance = data.OreBonusChance;
                miningGoddessNPC.CarryCapacity = data.CarryCapacity;
                miningGoddessNPC.HasLight = data.HasLight;

                npc.GivenName = data.Name;
            }

            Main.NewText(
                $"{data.Name} を召喚しました。",
                100, 255, 100
            );
        }

        //desummon 
        public static void DespawnMiner(int id)
        {
            foreach (NPC npc in Main.npc)
            {
                if (!npc.active)
                    continue;

                // 通常Miner
                if (npc.ModNPC is MinerNPC miner &&
                    miner.MinerID == id)
                {
                    npc.active = false;
                    return;
                }

                // MagicMiner
                if (npc.ModNPC is MagicMinerNPC magicMiner &&
                    magicMiner.MinerID == id)
                {
                    npc.active = false;
                    return;
                }

                // MagicMiner
                if (npc.ModNPC is MiningGoddessNPC goddessNPC &&
                    goddessNPC.MinerID == id)
                {
                    npc.active = false;
                    return;
                }
            }
        }
    }
}