using MinerManagementMOD.Common.UI;
using MinerManagementMOD.Systems;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MinerManagementMOD.NPCs
{
    [AutoloadHead]
    public class OreMerchant : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = Main.npcFrameCount[NPCID.Guide];
        }

        public override void SetDefaults()
        {
            NPC.width = 18;
            NPC.height = 40;

            NPC.townNPC = true;
            NPC.friendly = true;

            NPC.aiStyle = 7;

            AIType = NPCID.Guide;
            AnimationType = NPCID.Guide;

            NPC.damage = 0;
            NPC.defense = 9999;
            NPC.lifeMax = 9999;
            NPC.dontTakeDamage = true;
        }

        public override bool CanChat()
        {
            return true;
        }

        public override string GetChat()
        {
            return
                $"こんにちは。\n\n" +
                $"{MarketSystem.GetTop5Summary()}\n\n" +
                "今日は何を持ってきてくれたんだい？";
        }

        public override void SetChatButtons(
            ref string button,
            ref string button2)
        {
            button = "鉱物を売る";
            button2 = "納品";
        }

        public override void OnChatButtonClicked(bool firstButton, ref string shopName)
        {
            if (firstButton)
            {
                ModContent.GetInstance<MarketUISystem>()
                    .ShowUI(NPC.whoAmI);
            }
            else
            {
                DeliverySystem.OpenQuest(Main.LocalPlayer);
            }
        }

    }
}