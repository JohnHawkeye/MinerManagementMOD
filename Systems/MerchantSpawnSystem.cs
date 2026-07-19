using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using MinerManagementMOD.NPCs;
using MinerManagementMOD.Common.NPCs;

namespace MinerManagementMOD.Systems
{
    public class MerchantSpawnSystem : ModSystem
    {
        public override void PostUpdateNPCs()
        {
            RemoveDuplicateMerchants();
        }

        public static void SpawnMerchant()
        {
            if (MerchantSystem.MerchantAlive)
                return;

            // ワールドスポーン地点
            Vector2 spawnPosition = new Vector2(
                Main.spawnTileX * 16,
                (Main.spawnTileY - 2) * 16
            );

            int npcIndex = NPC.NewNPC(
                new EntitySource_WorldEvent(),
                (int)spawnPosition.X,
                (int)spawnPosition.Y,
                ModContent.NPCType<OreMerchant>()
            );
            NPC merchant = Main.npc[npcIndex];
            merchant.GetGlobalNPC<MinerGlobalNPC>().IsMarketMerchant = true;

            DeliverySystem.GenerateQuest();

            MerchantSystem.MerchantNPCIndex = npcIndex;
        }

        public static void RemoveMerchant()
        {
            if (!MerchantSystem.MerchantAlive)
                return;

            NPC npc = Main.npc[MerchantSystem.MerchantNPCIndex];

            Main.NewText($"{MerchantSystem.MerchantName}「私は帰ります。またよろしく！」");

            npc.active = false;
            npc.netUpdate = true;

            MerchantSystem.MerchantNPCIndex = -1;
            MerchantSystem.RemoveMerchant();
        }


        public static void RemoveDuplicateMerchants()
        {
            bool found = false;


            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];


                if (!npc.active)
                    continue;


                if (npc.GetGlobalNPC<MinerGlobalNPC>()
                    .IsMarketMerchant)
                {

                    if (!found)
                    {
                        // 最初の一人を残す
                        found = true;

                        MerchantSystem.MerchantNPCIndex = i;
                    }
                    else
                    {
                        // 2人目以降削除
                        npc.active = false;
                        npc.netUpdate = true;
                    }
                }
            }

        }
    }
}