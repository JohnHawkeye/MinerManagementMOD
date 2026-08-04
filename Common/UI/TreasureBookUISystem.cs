using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace MinerManagementMOD.Common.UI
{
    public class TreasureBookUISystem : ModSystem
    {
        internal static TreasureBookUISystem Instance;

        internal UserInterface TreasureBookInterface;
        internal TreasureBookUI TreasureBookUI;

        public static bool Visible;

        private Asset<Texture2D> treasureButtonTexture;

        public override void Load()
        {
            Instance = this;
            Visible = false;

            if (!Main.dedServ)
            {
                TreasureBookUI = new TreasureBookUI();
                TreasureBookUI.Activate();

                TreasureBookInterface = new UserInterface();
                TreasureBookInterface.SetState(TreasureBookUI);

                treasureButtonTexture = ModContent.Request<Texture2D>(
                    "MinerManagementMOD/Assets/UI/TreasureBookIcon",
                    AssetRequestMode.ImmediateLoad);
            }
        }

        public override void UpdateUI(GameTime gameTime)
        {
            if (!Visible)
                return;

            if (Main.keyState.IsKeyDown(Keys.Escape) &&
                Main.oldKeyState.IsKeyUp(Keys.Escape))
            {
                Visible = false;
                return;
            }
            TreasureBookInterface?.Update(gameTime);
        }

        public override void ModifyInterfaceLayers(System.Collections.Generic.List<GameInterfaceLayer> layers)
        {
            int inventoryLayer = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));

            if (inventoryLayer != -1)
            {
                layers.Insert(inventoryLayer, new LegacyGameInterfaceLayer(
                    "MinerManagementMOD: Treasure Book",
                    delegate
                    {
                        DrawTreasureButton();
                        if (Visible)
                        {
                            TreasureBookInterface.Draw(Main.spriteBatch, new GameTime());
                        }

                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }

        private void DrawTreasureButton()
        {
            if (!Main.playerInventory)
                return;

            // 鉱夫名簿ボタンの上に表示
            int x = Main.screenWidth - 260;
            int y = Main.screenHeight - 108;

            Rectangle rect = new Rectangle(x, y, 40, 40);

            Color color = Color.White;

            if (rect.Contains(Main.MouseScreen.ToPoint()))
            {
                color = Color.LightYellow;

                Main.LocalPlayer.mouseInterface = true;
                Main.hoverItemName = "トレジャー図鑑";

                if (Main.mouseLeft && Main.mouseLeftRelease)
                {
                    Visible = !Visible;
                    SoundEngine.PlaySound(SoundID.MenuTick);
                }
            }

            Main.spriteBatch.Draw(treasureButtonTexture.Value, rect, color);
        }
        
    }
}