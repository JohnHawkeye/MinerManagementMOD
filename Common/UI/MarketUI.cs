using Terraria.UI;
using Terraria.GameContent.UI.Elements;
using Microsoft.Xna.Framework;
using Terraria;
using MinerManagementMOD.Systems;

namespace MinerManagementMOD.Common.UI
{
    public class MarketUI : UIState
    {
        public UIPanel Panel;
        public MarketItemSlot ItemSlot;

        private UIText itemNameText;
        private UIText rateText;
        private UIText priceText;
        private UITextPanel<string> sellButton;

        public override void OnInitialize()
        {
            //panel
            Panel = new UIPanel();

            Panel.Width.Set(300, 0f);
            Panel.Height.Set(320, 0f);

            Panel.Left.Set(224, 0f);
            Panel.Top.Set(288, 0f);

            Append(Panel);

            //item slot
            ItemSlot = new MarketItemSlot();

            ItemSlot.Left.Set(124, 0f);
            ItemSlot.Top.Set(50, 0f);

            Panel.Append(ItemSlot);

            //item name
            itemNameText = new UIText("");
            itemNameText.Left.Set(20, 0);
            itemNameText.Top.Set(120, 0);
            Panel.Append(itemNameText);

            //rate
            rateText = new UIText("");
            rateText.Left.Set(20, 0);
            rateText.Top.Set(144, 0);
            Panel.Append(rateText);

            //price
            priceText = new UIText("売値：0");
            priceText.Left.Set(20, 0);
            priceText.Top.Set(168, 0);
            Panel.Append(priceText);

            //button
            sellButton = new UITextPanel<string>("売却する");
            sellButton.Width.Set(120, 0);
            sellButton.Height.Set(40, 0);

            sellButton.Left.Set(90, 0);
            sellButton.Top.Set(192, 0);

            sellButton.OnLeftClick += SellButtonClicked;

            Panel.Append(sellButton);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (ItemSlot.SlotItem.IsAir)
            {
                itemNameText.SetText("");
                rateText.SetText("");
                priceText.SetText("売値：0");
                return;
            }

            Item item = ItemSlot.SlotItem;
            itemNameText.SetText($"{item.Name} x{item.stack}");

            float rate = MarketSystem.GetRate(item.type);
            rateText.SetText($"現在相場：{rate * 100f:F0}%");

            long price = MarketSystem.GetSellPrice(item);

            priceText.SetText($"売値：{FormatCoins(price)}");
        }


        private string FormatCoins(long value)
        {
            int platinum = (int)(value / 1000000);
            value %= 1000000;

            int gold = (int)(value / 10000);
            value %= 10000;

            int silver = (int)(value / 100);
            int copper = (int)(value % 100);

            string text = "";

            if (platinum > 0)
                text += $"{platinum}プラチナ ";

            if (gold > 0)
                text += $"{gold}ゴールド ";

            if (silver > 0)
                text += $"{silver}シルバー ";

            text += $"{copper}カッパー";

            return text;
        }

        private void SellButtonClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            if (ItemSlot.SlotItem.IsAir)
            {
                Main.NewText("売却するアイテムがありません。");
                return;
            }

            bool result = MarketSystem.SellItem(
                Main.LocalPlayer,
                ItemSlot.SlotItem
            );

            if (result)
            {
                Main.NewText("アイテムを売却しました。");
            }
        }
        public void ReturnStoredItem()
        {
            ItemSlot?.ReturnItemToPlayer();
        }
    }
}