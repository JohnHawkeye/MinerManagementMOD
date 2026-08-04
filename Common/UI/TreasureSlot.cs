using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MinerManagementMOD.Systems;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.UI;

namespace MinerManagementMOD.Common.UI
{
    public class TreasureSlot : UIPanel
    {
        public int TreasureID { get; private set; }

        private bool hovering;

        public TreasureSlot(int id)
        {
            TreasureID = id;

            Width.Set(40f, 0f);
            Height.Set(40f, 0f);

            SetPadding(0);

            OnMouseOver += TreasureSlot_OnMouseOver;
            OnMouseOut += TreasureSlot_OnMouseOut;
            OnLeftClick += TreasureSlot_OnClick;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            CalculatedStyle dimensions = GetDimensions();

            // 未発見
            if (!TreasureBookSystem.IsDiscovered(TreasureID))
            {
                Utils.DrawBorderString(
                    spriteBatch,
                    "?",
                    new Vector2(
                        dimensions.X + 13,
                        dimensions.Y + 8),
                    Color.White);

                return;
            }

            // 発見済み
            // 現在は仮でアイテムIDから表示
            int itemType = TreasureBookSystem.GetItemType(TreasureID);
            //Main.NewText($"TreasureID:{TreasureID} ItemType:{itemType}");

            Texture2D texture = TextureAssets.Item[itemType].Value;
            Rectangle source = texture.Bounds;
            float scale = 32f / source.Width;

            if (source.Height > source.Width)
            {
                scale = 32f / source.Height;
            }

            Vector2 position = new Vector2(
                dimensions.X + 20,
                dimensions.Y + 20);

            spriteBatch.Draw(
                texture,
                position,
                source,
                Color.White,
                0f,
                source.Size() / 2f,
                scale,
                SpriteEffects.None,
                0f);
        }

        private void TreasureSlot_OnMouseOver(UIMouseEvent evt, UIElement listeningElement)
        {
            hovering = true;

            Main.LocalPlayer.mouseInterface = true;

            if (!TreasureBookSystem.IsDiscovered(TreasureID))
            {
                Main.hoverItemName = "？？？";
                return;
            }

            int itemType =
                TreasureBookSystem.GetItemType(TreasureID);

            Item item = ContentSamples.ItemsByType[itemType];
            Main.hoverItemName = item.Name;
        }

        private void TreasureSlot_OnMouseOut(UIMouseEvent evt, UIElement listeningElement)
        {
            hovering = false;
        }

        private void TreasureSlot_OnClick(
            UIMouseEvent evt,
            UIElement listeningElement)
        {
            if (!TreasureBookSystem.IsDiscovered(TreasureID))
                return;

            int itemType =
                TreasureBookSystem.GetItemType(TreasureID);

            TreasureBookUISystem.Instance
                .TreasureBookUI
                .ShowDetail(itemType);
        }

    }
}