using Microsoft.Xna.Framework;
using MinerManagementMOD.Systems;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace MinerManagementMOD.Common.UI
{
    public class TreasureBookUI : UIState
    {
        public UIPanel Panel;
        private UITextPanel<string> closeButton;
        private TreasureDetailUI detailUI;
        private UIText progressText;

        public override void OnInitialize()
        {
            Panel = new UIPanel();

            Panel.Width.Set(600f, 0f);
            Panel.Height.Set(764f, 0f);

            Panel.Left.Set(580f, 0);
            Panel.Top.Set(100f, 0);

            Append(Panel);

            //title
            UIText title = new UIText("トレジャーアイテム図鑑");
            title.Left.Set(20f, 0f);
            title.Top.Set(15f, 0f);

            Panel.Append(title);
            closeButton = new UITextPanel<string>("×");

            closeButton.Width.Set(40, 0);
            closeButton.Height.Set(40, 0);

            closeButton.Left.Set(560f, 0);
            closeButton.Top.Set(15f, 0);

            closeButton.OnLeftClick += CloseWindow;

            Panel.Append(closeButton);

            CreateSlots();

            progressText = new UIText("");

            progressText.Left.Set(250f, 0f);
            progressText.Top.Set(660f, 0f);

            Panel.Append(progressText);

            detailUI = new TreasureDetailUI();
            Append(detailUI);
            detailUI.Hide();

            UpdateProgressText();

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            UpdateProgressText();
        }

        public void ShowDetail(int itemType)
        {
            detailUI.SetTreasure(itemType);

            detailUI.Show();
        }

        public void HideDetail()
        {
            detailUI.Hide();
        }

        private void CloseWindow(UIMouseEvent evt, UIElement listeningElement)
        {
            TreasureBookUISystem.Visible = false;
        }

        private void CreateSlots()
        {
            const int columns = 10;
            const int size = 48;


            for (int i = 0; i < 128; i++)
            {
                int x = i % columns;
                int y = i / columns;

                TreasureSlot slot = new TreasureSlot(i + 1);

                slot.Left.Set(
                    20 + x * size,
                    0);

                slot.Top.Set(
                    60 + y * size,
                    0);

                Panel.Append(slot);
            }
        }

        private void UpdateProgressText()
        {
            int discovered = TreasureBookSystem.GetDiscoveredCount();

            if (discovered >= TreasureBookSystem.TreasureCount)
            {
                progressText.TextColor = Color.Gold;
                progressText.SetText("★★★★★ Complete!! ★★★★★");
            }
            else
            {
                progressText.TextColor = Color.White;
                progressText.SetText($"{discovered} / {TreasureBookSystem.TreasureCount}");
            }
        }
    }
}