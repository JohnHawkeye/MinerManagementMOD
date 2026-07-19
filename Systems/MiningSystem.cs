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

            // 既に採掘員がいるなら何もしない
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (npc.type == ModContent.NPCType<MinerNPC>())
                {
                    minerSpawned = true;
                    return;
                }
            }

            IEntitySource source = new EntitySource_Misc("MinerSpawn");

            NPC.NewNPC(
                source,
                (int)player.Center.X + 64,
                (int)player.Center.Y,
                ModContent.NPCType<MinerNPC>());

            NPC.NewNPC(
                source,
                (int)player.Center.X + 64,
                (int)player.Center.Y,
                ModContent.NPCType<GuardNPC>());

            minerSpawned = true;
        }
    }
}