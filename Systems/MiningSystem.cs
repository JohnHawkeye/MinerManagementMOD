using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using MinerManagementMOD.NPCs;

namespace MinerManagementMOD.Systems
{
    public class MinerSystem : ModSystem
    {
        private bool minerSpawned;

        private int hunterRespawnTimer = -1;
        private int guardRespawnTimer = -1;

        public override void OnWorldLoad()
        {
            minerSpawned = false;
        }

        public override void OnWorldUnload()
        {
            minerSpawned = false;
        }

        public override void PreUpdateWorld()
        {
            // サーバー側のみでスポーン処理
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            if (minerSpawned)
                return;

            Player player = Main.LocalPlayer;

            if (player == null || !player.active || player.dead)
                return;

            // いるなら何もしない
            bool hunterAlive = false;
            bool guardAlive = false;

            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (npc.type == ModContent.NPCType<HunterNPC>())
                {
                    hunterAlive = true;
                }
                if (npc.type == ModContent.NPCType<GuardNPC>())
                {
                    guardAlive = true;
                }
            }
            if (!hunterAlive)
            {
                if (hunterRespawnTimer == -1)
                    hunterRespawnTimer = 300; //5秒
                else if (--hunterRespawnTimer <= 0)
                {
                    NPC.NewNPC(
                        new EntitySource_Misc("HunterRespawn"),
                        (int)player.Center.X,
                        (int)player.Center.Y,
                        ModContent.NPCType<HunterNPC>());

                    hunterRespawnTimer = -1;
                }
            }
            else
            {
                hunterRespawnTimer = -1;
            }
            
            if (!guardAlive)
            {
                if (guardRespawnTimer == -1)
                    guardRespawnTimer = 300;
                else if (--guardRespawnTimer <= 0)
                {
                    NPC.NewNPC(
                        new EntitySource_Misc("GuardRespawn"),
                        (int)player.Center.X,
                        (int)player.Center.Y,
                        ModContent.NPCType<GuardNPC>());

                    guardRespawnTimer = -1;
                }
            }
            else
            {
                guardRespawnTimer = -1;
            }
        }
    }
}