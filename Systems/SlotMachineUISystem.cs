using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using MinerManagementMOD.UI;
using Microsoft.Xna.Framework;
using log4net.DateFormatter;

namespace MinerManagementMOD.Systems
{
    public class SlotMachineUISystem : ModSystem
    {
        public static bool IsOpen {get;private set;}

        internal SlotMachineUI SlotUI;
        private UserInterface userInterface;

        private Point slotMachinePosition;

        private const float CloseDistance = 64f;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                SlotUI = new SlotMachineUI();
                SlotUI.Activate();

                userInterface = new UserInterface();
            }
        }

        public override void UpdateUI(GameTime gameTime)
        {
            if (userInterface?.CurrentState != null)
            {
                userInterface.Update(Main._drawInterfaceGameTime);

                Player player = Main.LocalPlayer;

                // スロットマシーンの中心座標
                Vector2 slotCenter = new Vector2(
                    slotMachinePosition.X * 16f + 16f,
                    slotMachinePosition.Y * 16f + 32f
                );

                // プレイヤーとスロットマシーンの距離
                float distance = Vector2.Distance(
                    player.Center,
                    slotCenter
                );

                // 離れたら閉じる
                if (distance > CloseDistance)
                {
                    Hide();
                }
            }
        }

        public override void ModifyInterfaceLayers(
            System.Collections.Generic.List<GameInterfaceLayer> layers)
        {
            int inventoryLayer = layers.FindIndex(
                layer => layer.Name.Equals("Vanilla: Mouse Text")
            );

            if (inventoryLayer != -1)
            {
                layers.Insert(
                    inventoryLayer,
                    new LegacyGameInterfaceLayer(
                        "MinerManagementMOD: Slot Machine UI",
                        delegate
                        {
                            if (userInterface?.CurrentState != null)
                            {
                                userInterface.Draw(
                                    Main.spriteBatch,
                                    Main._drawInterfaceGameTime
                                );
                            }

                            return true;
                        },
                        InterfaceScaleType.UI
                    )
                );
            }
        }

        public void Show(int tileX,int tileY)
        {
            slotMachinePosition = new Point(tileX,tileY);
            userInterface?.SetState(SlotUI);
            IsOpen = true;
        }

        public void Hide()
        {
            userInterface?.SetState(null);
            IsOpen =false;
        }

    }
}