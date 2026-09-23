using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using MinerManagementMOD.Common.Players;

namespace MinerManagementMOD.Systems
{
    public class MossHornetDrawLayer : PlayerDrawLayer
    {
        private Asset<Texture2D> texture;


        // =========================================================
        // 表示条件
        // =========================================================

        public override bool GetDefaultVisibility(
            PlayerDrawSet drawInfo)
        {
            return drawInfo.drawPlayer
                .GetModPlayer<MossHornetPlayer>()
                .MossHornetActive;
        }


        // =========================================================
        // 描画位置
        // =========================================================

        public override Position GetDefaultPosition()
        {
            return PlayerDrawLayers.AfterLastVanillaLayer;
        }


        // =========================================================
        // 描画
        // =========================================================

        protected override void Draw(
            ref PlayerDrawSet drawInfo)
        {
            if (drawInfo.shadow != 0f)
                return;

            Player player =
                drawInfo.drawPlayer;

            MossHornetPlayer hornet =
                player.GetModPlayer<MossHornetPlayer>();

            if (!hornet.MossHornetActive)
                return;


            // =====================================================
            // テクスチャ読み込み
            // =====================================================

            texture ??=
                ModContent.Request<Texture2D>(
                    "MinerManagementMOD/Assets/Mounts/MossHornetMount"
                );


            // =====================================================
            // アニメーション
            // =====================================================

            int frame =
                (int)(Main.GameUpdateCount / 8 % 2);


            Rectangle sourceRectangle =
                new Rectangle(
                    0,
                    frame * 47,
                    48,
                    47
                );


            // =====================================================
            // 左右反転
            // =====================================================

            SpriteEffects effects =
                player.direction == -1
                    ? SpriteEffects.FlipHorizontally
                    : SpriteEffects.None;


            // =====================================================
            // 描画
            // =====================================================

            drawInfo.DrawDataCache.Add(
                new DrawData(
                    texture.Value,
                    player.Center - Main.screenPosition,
                    sourceRectangle,
                    Color.White,
                    0f,
                    new Vector2(24f, 23.5f),
                    1f,
                    effects,
                    0
                )
            );
        }
    }
}