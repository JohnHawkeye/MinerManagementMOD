using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;

namespace MinerManagementMOD.Common
{
    public class GlobalNPCDrops : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            // 海のカニ
            if (npc.type == NPCID.Crab)
            {
                // カニのはさみ 10%ドロップ
                npcLoot.Add(
                    ItemDropRule.Common(
                        ModContent.ItemType<Items.CrabClaw>(),
                        2
                    )
                );
            }
        }
    }
}