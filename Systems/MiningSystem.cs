using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using MinerManagementMOD.NPCs;

namespace MinerManagementMOD.Systems
{
    public class MinerSystem : ModSystem
    {
        // =========================================================
        // 特殊NPC召喚 ON / OFF
        // =========================================================

        public static bool SpecialNPCEnabled = true;

        // 再召喚タイマー
        private int hunterRespawnTimer = -1;
        private int guardRespawnTimer = -1;
        private int healerRespawnTimer = -1;


        // =========================================================
        // ワールド開始
        // =========================================================

        public override void OnWorldLoad()
        {
            SpecialNPCEnabled = true;

            hunterRespawnTimer = -1;
            guardRespawnTimer = -1;
            healerRespawnTimer = -1;
        }


        // =========================================================
        // ワールド終了
        // =========================================================

        public override void OnWorldUnload()
        {
            SpecialNPCEnabled = true;

            hunterRespawnTimer = -1;
            guardRespawnTimer = -1;
            healerRespawnTimer = -1;
        }


        // =========================================================
        // 特殊NPC ON / OFF 切り替え
        // =========================================================

        public static void ToggleSpecialNPC()
        {
            SpecialNPCEnabled = !SpecialNPCEnabled;

            // OFFにした場合
            if (!SpecialNPCEnabled)
            {
                // MinerSystemのインスタンスを取得
                MinerSystem system =
                    ModContent.GetInstance<MinerSystem>();

                // 再召喚タイマーをリセット
                system.hunterRespawnTimer = -1;
                system.guardRespawnTimer = -1;
                system.healerRespawnTimer = -1;

                // 現在存在している特殊NPCを退場させる
                foreach (NPC npc in Main.ActiveNPCs)
                {
                    if (npc.type == ModContent.NPCType<HunterNPC>() ||
                        npc.type == ModContent.NPCType<GuardNPC>() ||
                        npc.type == ModContent.NPCType<HealerNPC>())
                    {
                        npc.active = false;
                    }
                }
            }
        }


        // =========================================================
        // ワールド更新
        // =========================================================

        public override void PreUpdateWorld()
        {
            // マルチプレイのクライアントでは処理しない
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return;

            // OFFなら何もしない
            if (!SpecialNPCEnabled)
                return;

            Player player = Main.LocalPlayer;

            if (player == null || !player.active || player.dead)
                return;


            // =====================================================
            // NPC存在確認
            // =====================================================

            bool hunterAlive = false;
            bool guardAlive = false;
            bool healerAlive = false;

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

                if (npc.type == ModContent.NPCType<HealerNPC>())
                {
                    healerAlive = true;
                }
            }


            // =====================================================
            // ハンター
            // =====================================================

            if (!hunterAlive)
            {
                if (hunterRespawnTimer == -1)
                {
                    hunterRespawnTimer = 300;
                }
                else if (--hunterRespawnTimer <= 0)
                {
                    NPC.NewNPC(
                        new EntitySource_Misc("HunterRespawn"),
                        (int)player.Center.X,
                        (int)player.Center.Y,
                        ModContent.NPCType<HunterNPC>()
                    );

                    hunterRespawnTimer = -1;
                }
            }
            else
            {
                hunterRespawnTimer = -1;
            }


            // =====================================================
            // ガード
            // =====================================================

            if (!guardAlive)
            {
                if (guardRespawnTimer == -1)
                {
                    guardRespawnTimer = 300;
                }
                else if (--guardRespawnTimer <= 0)
                {
                    NPC.NewNPC(
                        new EntitySource_Misc("GuardRespawn"),
                        (int)player.Center.X,
                        (int)player.Center.Y,
                        ModContent.NPCType<GuardNPC>()
                    );

                    guardRespawnTimer = -1;
                }
            }
            else
            {
                guardRespawnTimer = -1;
            }


            // =====================================================
            // ヒーラー
            // =====================================================

            if (!healerAlive)
            {
                if (healerRespawnTimer == -1)
                {
                    healerRespawnTimer = 300;
                }
                else if (--healerRespawnTimer <= 0)
                {
                    NPC.NewNPC(
                        new EntitySource_Misc("HealerRespawn"),
                        (int)player.Center.X,
                        (int)player.Center.Y,
                        ModContent.NPCType<HealerNPC>()
                    );

                    healerRespawnTimer = -1;
                }
            }
            else
            {
                healerRespawnTimer = -1;
            }
        }
    }
}