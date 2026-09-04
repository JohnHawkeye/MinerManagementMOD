using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace MinerManagementMOD.Systems
{
    public class StoneWandPreviewSystem : ModSystem
    {
        // プレビューの色
        // A値を小さくするほど透明になります
        private static readonly Color PreviewColor =
            new Color(190, 190, 190, 90);

        private const int TileSize = 16;

        public override void PostDrawTiles()
        {
            Player player = Main.LocalPlayer;

            if (player == null || !player.active)
                return;

            // StoneWandを持っているときだけ表示
            if (player.HeldItem == null ||
                player.HeldItem.type != ModContent.ItemType<Items.StoneWand>())
            {
                return;
            }

            // Stoneを持っていない場合はプレビューを表示しない
            if (player.CountItem(Terraria.ID.ItemID.StoneBlock) <= 0)
                return;

            DrawPreview();
        }

        private void DrawPreview()
        {
            Point center = Main.MouseWorld.ToTileCoordinates();

            // 現在のSpriteBatchを開始
            Main.spriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                DepthStencilState.None,
                RasterizerState.CullNone,
                null,
                Main.GameViewMatrix.TransformationMatrix
            );

            for (int offsetX = -1; offsetX <= 1; offsetX++)
            {
                for (int offsetY = -1; offsetY <= 1; offsetY++)
                {
                    int tileX = center.X + offsetX;
                    int tileY = center.Y + offsetY;

                    if (!WorldGen.InWorld(tileX, tileY, 1))
                        continue;

                    Tile tile = Framing.GetTileSafely(tileX, tileY);

                    // すでにタイルがある場所は表示しない
                    if (tile.HasTile)
                        continue;

                    // 液体がある場所も表示しない
                    if (tile.LiquidAmount > 0)
                        continue;

                    DrawTilePreview(tileX, tileY);
                }
            }

            // SpriteBatchを終了
            Main.spriteBatch.End();
        }

        private void DrawTilePreview(int tileX, int tileY)
        {
            Texture2D texture = TextureAssets.MagicPixel.Value;

            Vector2 worldPosition = new Vector2(
                tileX * TileSize,
                tileY * TileSize
            );

            Vector2 screenPosition =
                worldPosition - Main.screenPosition;

            Main.spriteBatch.Draw(
                texture,
                new Rectangle(
                    (int)screenPosition.X + 1,
                    (int)screenPosition.Y + 1,
                    TileSize - 2,
                    TileSize - 2
                ),
                PreviewColor
            );
        }
    }
}