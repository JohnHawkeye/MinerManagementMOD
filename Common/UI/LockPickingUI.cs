using Terraria;
using Terraria.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using Terraria.ID;
using System.Security;


namespace MinerManagementMOD.UI
{
    public class LockPickingUI : UIState
    {
        protected override void DrawSelf(
            SpriteBatch spriteBatch)
        {
            Player player =
                Main.LocalPlayer;

            Vector2 pos =
                player.Center -
                Main.screenPosition;

            // 頭上へ移動
            pos.Y -= 80;

            DrawBubble(
                spriteBatch,
                pos
            );
        }

        private void DrawBubble(

            SpriteBatch spriteBatch,
            Vector2 center)
        {
            Texture2D pixel =
                Terraria.GameContent
                .TextureAssets.MagicPixel.Value;

            // 背景
            spriteBatch.Draw(
                pixel,
                new Rectangle(
                    (int)center.X - 40,
                    (int)center.Y - 40,
                    80,
                    80),
                Color.SkyBlue * 0.6f
            );

            int radius = 25;


            // 円
            for (int i = 0; i < 360; i++)
            {
                float a =
                    MathHelper.ToRadians(i);

                Vector2 p =
                    center +
                    new Vector2(
                        (float)System.Math.Cos(a),
                        (float)System.Math.Sin(a)
                    )
                    * radius;

                spriteBatch.Draw(
                    pixel,
                    new Rectangle(
                        (int)p.X,
                        (int)p.Y,
                        2,
                        2),
                    Color.White
                );
            }

            //save range
            var player =
                Main.LocalPlayer.GetModPlayer<Players.LockPickingPlayer>();

            for (
                float i = player.SuccessStartAngle;
                i <= player.SuccessEndAngle;
                i += 2)
            {
                float a =
                    MathHelper.ToRadians(i);

                Vector2 p =
                    center +
                    new Vector2(
                        (float)System.Math.Cos(a),
                        (float)System.Math.Sin(a))
                    * radius;

                spriteBatch.Draw(
                    pixel,
                    new Rectangle(
                        (int)p.X,
                        (int)p.Y,
                        3,
                        3),
                    Color.Green);
            }

            // 赤い点

            float rad =
                MathHelper.ToRadians(
                    player.NeedleAngle
                );


            Vector2 needle =
                center +
                new Vector2(
                    (float)System.Math.Cos(rad),
                    (float)System.Math.Sin(rad)
                )
                * radius;

            spriteBatch.Draw(
                pixel,
                new Rectangle(
                    (int)needle.X - 4,
                    (int)needle.Y - 4,
                    8,
                    8),
                Color.Red
            );
        }
    }
}