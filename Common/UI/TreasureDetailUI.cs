using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace MinerManagementMOD.Common.UI
{
    public class TreasureDetailUI : UIPanel
    {
        private UIText nameText;
        private UIText valueText;
        private UIText descriptionText;

        public bool Visible { get; private set; }

        public TreasureDetailUI()
        {
            Width.Set(300, 0);
            Height.Set(180, 0);

            Left.Set(150, 0);
            Top.Set(320, 0);


            nameText = new UIText("");
            nameText.Left.Set(20, 0);
            nameText.Top.Set(20, 0);

            Append(nameText);


            valueText = new UIText("");
            valueText.Left.Set(20, 0);
            valueText.Top.Set(60, 0);

            Append(valueText);


            descriptionText = new UIText("");
            descriptionText.Left.Set(20, 0);
            descriptionText.Top.Set(100, 0);

            Append(descriptionText);
            Visible = false;
        }


        public void SetTreasure(int itemType)
        {
            Item item = new Item();
            item.SetDefaults(itemType);


            nameText.SetText(
                item.Name);


            valueText.SetText(
                $"売却価格 : {item.value}");


            descriptionText.SetText(
                "トレジャーアイテム");
        }

        protected override void DrawSelf(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            if (!Visible)
                return;

            base.DrawSelf(spriteBatch);
        }

        public void Show()
        {
            Visible = true;
        }

        public void Hide()
        {
            Visible = false;
        }
    }
}