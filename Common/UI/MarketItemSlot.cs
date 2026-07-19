using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.UI;
using MinerManagementMOD.Market;

namespace MinerManagementMOD.Common.UI
{
    public class MarketItemSlot : UIElement
    {
        public Item SlotItem;

        public MarketItemSlot()
        {
            Width.Set(52f, 0f);
            Height.Set(52f, 0f);

            SlotItem = new Item();
            SlotItem.TurnToAir();
        }

        public override void Update(GameTime gameTime)
        {
            Main.LocalPlayer.mouseInterface = true;

            base.Update(gameTime);
        }

        public override bool ContainsPoint(Vector2 point)
        {
            return GetDimensions().ToRectangle().Contains(point.ToPoint());
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            Main.LocalPlayer.mouseInterface = true;
            Item mouseItem = Main.mouseItem;

            // スロットが空で、カーソルにアイテムがある
            if (SlotItem.IsAir && !mouseItem.IsAir)
            {
                if(!MarketDatabase.CanTrade(mouseItem))
                    return;

                SlotItem = mouseItem.Clone();

                // カーソルアイテムを完全消去
                Main.mouseItem.TurnToAir();

                return;
            }


            // スロットから取り出す
            if (!SlotItem.IsAir && mouseItem.IsAir)
            {
                Main.mouseItem = SlotItem.Clone();

                SlotItem.TurnToAir();

                return;
            }


            // 両方にアイテムがある場合は交換
            if (!SlotItem.IsAir && !mouseItem.IsAir)
            {

                if(!MarketDatabase.CanTrade(mouseItem))
                    return;

                Item temp = SlotItem.Clone();

                SlotItem = mouseItem.Clone();

                Main.mouseItem = temp;

                return;
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            CalculatedStyle dim = GetDimensions();

            spriteBatch.Draw(
                TextureAssets.InventoryBack.Value,
                dim.Position(),
                Color.White);

            if (!SlotItem.IsAir)
            {
                Main.instance.LoadItem(SlotItem.type);

                Texture2D tex = TextureAssets.Item[SlotItem.type].Value;

                Rectangle frame = tex.Frame();

                float scale = 1f;

                if (frame.Width > 32 || frame.Height > 32)
                    scale = 32f / System.Math.Max(frame.Width, frame.Height);

                spriteBatch.Draw(
                    tex,
                    dim.Position() + new Vector2(26),
                    frame,
                    Color.White,
                    0,
                    frame.Size() / 2,
                    scale,
                    SpriteEffects.None,
                    0);

                if (SlotItem.stack > 1)
                {
                    Utils.DrawBorderString(
                        spriteBatch,
                        SlotItem.stack.ToString(),
                        dim.Position() + new Vector2(30, 28),
                        Color.White);
                }
            }
        }
        /// <summary>
        /// スロット内のアイテムをプレイヤーへ返却する
        /// </summary>
        public void ReturnItemToPlayer()
        {
            if (SlotItem.IsAir)
                return;

            Player player = Main.LocalPlayer;

            int itemType = SlotItem.type;
            int itemStack = SlotItem.stack;

            player.QuickSpawnItem(
                player.GetSource_Misc("MarketClose"),
                itemType,itemStack
            );

            SlotItem.TurnToAir();
        }
    }
}