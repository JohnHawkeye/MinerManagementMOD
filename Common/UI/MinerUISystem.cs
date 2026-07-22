using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace MinerManagementMOD.Common.UI
{
    public class MinerUISystem : ModSystem
    {
        internal static MinerUISystem Instance;

        internal UserInterface MinerInterface;
        internal MinerRosterUI MinerUI;

        public static bool Visible;
        private Asset<Texture2D> minerButtonTexture;

        public override void Load()
        {

            Instance = this;

            if (!Main.dedServ)
            {
                MinerUI = new MinerRosterUI();
                MinerUI.Activate();

                MinerInterface = new UserInterface();
                MinerInterface.SetState(MinerUI);

                minerButtonTexture = ModContent.Request<Texture2D>(
                                        "MinerManagementMOD/Assets/UI/RoserIcon",
                                        AssetRequestMode.ImmediateLoad);
            }
        }

        public override void UpdateUI(GameTime gameTime)
        {
            if (!Visible)
                return;
            
            if(Main.keyState.IsKeyDown(Keys.Escape)&&
                Main.oldKeyState.IsKeyUp(Keys.Escape))
            {
                Visible =false;
                return;   
            }

            MinerInterface?.Update(gameTime);
        }

        public override void ModifyInterfaceLayers(System.Collections.Generic.List<GameInterfaceLayer> layers)
        {
            int inventoryLayer = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));

            if (inventoryLayer != -1)
            {
                layers.Insert(inventoryLayer, new LegacyGameInterfaceLayer(
                    "MinerManagementMOD: Miner UI",
                    delegate
                    {
                        DrawMinerButton();

                        if (Visible)
                        {
                            MinerInterface.Draw(Main.spriteBatch, new GameTime());
                        }

                        return true;
                    },
                    InterfaceScaleType.UI));
            }
        }

        private void DrawMinerButton()
        {
            if (!Main.playerInventory)
                return;

            // 「設定」の文字の左側付近
            int x = Main.screenWidth - 210;
            int y = Main.screenHeight - 64;

            Rectangle rect = new Rectangle(x, y, 40, 40);

            Color color = Color.White;

            if (rect.Contains(Main.MouseScreen.ToPoint()))
            {
                color = Color.LightYellow;

                Main.LocalPlayer.mouseInterface = true;
                Main.hoverItemName = "鉱夫名簿";

                if (Main.mouseLeft && Main.mouseLeftRelease)
                {
                    Visible = !Visible;
                    if (Visible)
                    {
                        MinerUI.RefreshPage();
                    }

                    SoundEngine.PlaySound(SoundID.MenuTick);
                }
            }

            Main.spriteBatch.Draw(minerButtonTexture.Value, rect, color);
        }
    }
}