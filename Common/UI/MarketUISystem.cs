using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace MinerManagementMOD.Common.UI
{
    public class MarketUISystem : ModSystem
    {
        internal UserInterface MarketInterface;
        internal MarketUI MarketUI;
        private static int openedNPC = -1;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                MarketUI = new MarketUI();
                MarketUI.Activate();

                MarketInterface = new UserInterface();
            }
        }

        public override void UpdateUI(GameTime gameTime)
        {
            if (MarketInterface?.CurrentState != null)
            {
                Main.playerInventory = true;
                Main.LocalPlayer.mouseInterface = true;
                CheckNPCDistance();
                MarketInterface.Update(gameTime);
            }
        }

        public override void ModifyInterfaceLayers(System.Collections.Generic.List<GameInterfaceLayer> layers)
        {
            int inventoryLayer = layers.FindIndex(
                layer => layer.Name.Equals("Vanilla: Inventory"));

            if (inventoryLayer != -1)
            {
                layers.Insert(
                    inventoryLayer,
                    new LegacyGameInterfaceLayer(
                        "MinerManagementMOD: Market UI",
                        delegate
                        {
                            if (MarketInterface?.CurrentState != null)
                            {
                                MarketInterface.Draw(
                                    Main.spriteBatch,
                                     new GameTime()
                                     );
                            }
                            return true;
                        },
                    InterfaceScaleType.UI
                    )
                );
            }
        }

        public void ShowUI(int npcWhoAmI)
        {
            openedNPC = npcWhoAmI;
            Main.playerInventory = true;
            MarketInterface?.SetState(MarketUI);
        }

        public void HideUI()
        {
            if(MarketInterface?.CurrentState is MarketUI marketUI)
            {
                marketUI.ReturnStoredItem();
            }
            MarketInterface?.SetState(null);
            openedNPC = -1;
        }


        private void CheckNPCDistance()
        {
            if (openedNPC < 0)
                return;


            NPC npc = Main.npc[openedNPC];


            // NPCが消滅した場合
            if (!npc.active)
            {
                HideUI();
                return;
            }


            // 会話中ではない場合
            if (Main.LocalPlayer.talkNPC != openedNPC)
            {
                HideUI();
                return;
            }


            // 距離チェック
            float distance =
                Vector2.Distance(
                    Main.LocalPlayer.Center,
                    npc.Center
                );


            if (distance > 200f)
            {
                HideUI();
            }
        }
    }
}