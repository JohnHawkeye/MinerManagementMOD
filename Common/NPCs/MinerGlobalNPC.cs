using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace MinerManagementMOD.Common.NPCs
{
    public class MinerGlobalNPC : GlobalNPC
    {
        // 市場商人タグ
        public bool IsMarketMerchant;

        public override bool InstancePerEntity => true;


        public override void SaveData(NPC npc, TagCompound tag)
        {
            if (IsMarketMerchant)
            {
                tag["IsMarketMerchant"] = true;
            }
        }


        public override void LoadData(NPC npc, TagCompound tag)
        {
            if (tag.ContainsKey("IsMarketMerchant"))
            {
                IsMarketMerchant = true;
            }
        }
    }
}