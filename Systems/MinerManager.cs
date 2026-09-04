using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using MinerManagementMOD.NPCs;

namespace MinerManagementMOD.Systems
{
    public static class MinerManager
    {
        // ============================================================
        // NPCが召喚されているか確認
        // ============================================================

        public static bool IsMinerSpawned(int id)
        {
            foreach (NPC npc in Main.npc)
            {
                if (!npc.active)
                    continue;

                // 通常Miner
                if (npc.ModNPC is MinerNPC miner &&
                    miner.MinerID == id)
                {
                    return true;
                }

                // MagicMiner
                if (npc.ModNPC is MagicMinerNPC magicMiner &&
                    magicMiner.MinerID == id)
                {
                    return true;
                }

                // Jewel Dragon
                if (id == 4 &&
                    npc.ModNPC is JewelDragonNPC)
                {
                    return true;
                }

                // Mining Goddess
                if (npc.ModNPC is MiningGoddessNPC miningGoddessNPC &&
                    miningGoddessNPC.MinerID == id)
                {
                    return true;
                }
            }

            return false;
        }


        // ============================================================
        // NPC召喚
        // ============================================================

        public static void SpawnMiner(Player player, MinerData data)
        {
            // --------------------------------------------------------
            // 既に存在する場合は召喚解除
            // --------------------------------------------------------

            if (IsMinerSpawned(data.ID))
            {
                DespawnMiner(data.ID);

                Main.NewText(
                    $"{data.Name} の召喚を解除しました。",
                    255, 200, 100
                );

                return;
            }


            // --------------------------------------------------------
            // NPCタイプを決定
            // --------------------------------------------------------

            int npcType;

            // 4番目：ジュエルドラゴン
            if (data.ID == 4)
            {
                npcType = ModContent.NPCType<JewelDragonNPC>();
            }
            // 5番目：採掘の女神
            else if (data.ID == 5)
            {
                npcType = ModContent.NPCType<MiningGoddessNPC>();
            }
            // 3番目：MagicMiner
            else if (data.ID == 3)
            {
                npcType = ModContent.NPCType<MagicMinerNPC>();
            }
            // その他：通常の鉱夫
            else
            {
                npcType = ModContent.NPCType<MinerNPC>();
            }


            // --------------------------------------------------------
            // NPC生成
            // --------------------------------------------------------

            int npcID = NPC.NewNPC(
                null,
                (int)player.Center.X,
                (int)player.Center.Y,
                npcType
            );

            NPC npc = Main.npc[npcID];


            // ========================================================
            // 通常Miner
            // ========================================================

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


            // ========================================================
            // MagicMiner
            // ========================================================

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


            // ========================================================
            // Jewel Dragon
            // ========================================================

            else if (npc.ModNPC is JewelDragonNPC jewelDragon)
            {
                jewelDragon.OwnerPlayer = player.whoAmI;
                npc.GivenName = data.Name;
                npc.netUpdate = true;
            }


            // ========================================================
            // Mining Goddess
            // ========================================================

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


            // --------------------------------------------------------
            // 召喚メッセージ
            // --------------------------------------------------------

            Main.NewText(
                $"{data.Name} を召喚しました。",
                100, 255, 100
            );
        }


        // ============================================================
        // 召喚解除
        // ============================================================

        public static void DespawnMiner(int id)
        {
            foreach (NPC npc in Main.npc)
            {
                if (!npc.active)
                    continue;


                // ----------------------------------------------------
                // 通常Miner
                // ----------------------------------------------------

                if (npc.ModNPC is MinerNPC miner &&
                    miner.MinerID == id)
                {
                    npc.active = false;
                    return;
                }


                // ----------------------------------------------------
                // MagicMiner
                // ----------------------------------------------------

                if (npc.ModNPC is MagicMinerNPC magicMiner &&
                    magicMiner.MinerID == id)
                {
                    npc.active = false;
                    return;
                }


                // ----------------------------------------------------
                // Jewel Dragon
                // ----------------------------------------------------

                if (id == 4 &&
                    npc.ModNPC is JewelDragonNPC)
                {
                    npc.active = false;
                    return;
                }


                // ----------------------------------------------------
                // Mining Goddess
                // ----------------------------------------------------

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