using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using Microsoft.Xna.Framework;
using MinerManagementMOD.UI;

namespace MinerManagementMOD.Systems
{
    public class LockPickingUISystem : ModSystem
    {
        public static UserInterface Interface;
        public static LockPickingUI UIState;


        public override void Load()
        {
            if (!Main.dedServ)
            {
                UIState = new LockPickingUI();
                UIState.Activate();

                Interface = new UserInterface();
            }
        }


        public override void UpdateUI(GameTime gameTime)
        {
            var player = Main.LocalPlayer;

            if (player == null)
                return;


            bool picking =
                player.GetModPlayer<Players.LockPickingPlayer>()
                .IsPicking;


            if(picking)
            {
                Interface.SetState(UIState);
            }
            else
            {
                Interface.SetState(null);
            }


            Interface?.Update(gameTime);
        }


        public override void ModifyInterfaceLayers(
            System.Collections.Generic.List<GameInterfaceLayer> layers)
        {
            int index =
                layers.FindIndex(
                    layer =>
                    layer.Name.Equals(
                    "Vanilla: Mouse Text")
                );


            if(index != -1)
            {
                layers.Insert(
                    index,
                    new LegacyGameInterfaceLayer(
                        "MMM: LockPicking",
                        delegate
                        {
                            Interface.Draw(
                                Main.spriteBatch,
                                new GameTime()
                            );

                            return true;
                        },
                        InterfaceScaleType.UI
                    )
                );
            }
        }
    }
}