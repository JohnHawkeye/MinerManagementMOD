using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using MinerManagementMOD.Systems;
using Terraria.ID;

namespace MinerManagementMOD.Common.UI
{
    public class MinerRosterUI : UIState
    {
        public UIPanel Panel;

        private UIText nameText;
        private UIText pageText;

        private UITextPanel<string> summonButton;
        private UITextPanel<string> leftButton;
        private UITextPanel<string> rightButton;
        private UITextPanel<string> closeButton;
        private UITextPanel<string> hireButton;

        private int currentPage = 0;

        public override void OnInitialize()
        {
            //window
            //----------------------------------------------------
            Panel = new UIPanel();
            Panel.Width.Set(600, 0);
            Panel.Height.Set(700, 0);

            Panel.Left.Set(640, 0);
            Panel.Top.Set(125, 0);

            Append(Panel);

            //------------------------------------------------
            //Hire
            //-----------------------------------------------
            hireButton = new UITextPanel<string>("雇う\n10G");

            hireButton.Width.Set(120, 0);
            hireButton.Height.Set(50, 0);

            hireButton.Left.Set(240, 0);
            hireButton.Top.Set(90, 0);

            hireButton.OnLeftClick += HireButtonClicked;

            Panel.Append(hireButton);


            //---------------------------------------------------
            // タイトル
            //---------------------------------------------------

            UIText title = new UIText("鉱夫名簿", 0.8f, true);
            title.HAlign = 0.5f;
            title.Top.Set(15, 0);

            Panel.Append(title);

            //---------------------------------------------------
            // 閉じる
            //---------------------------------------------------

            closeButton = new UITextPanel<string>("×");

            closeButton.Width.Set(40, 0);
            closeButton.Height.Set(40, 0);

            closeButton.Left.Set(550, 0);
            closeButton.Top.Set(10, 0);

            closeButton.OnLeftClick += CloseWindow;

            Panel.Append(closeButton);

            //---------------------------------------------------
            // 名前
            //---------------------------------------------------

            nameText = new UIText("");

            nameText.Left.Set(90, 0);
            nameText.Top.Set(95, 0);

            Panel.Append(nameText);

            //-----------------------------------------------------
            //summon button
            //-----------------------------------------------------
            summonButton = new UITextPanel<string>("召喚");
            summonButton.Left.Set(260, 0);
            summonButton.Top.Set(80, 0);
            summonButton.OnLeftClick += SummonButtonClicked;
            Panel.Append(summonButton);

            //---------------------------------------------------
            // ←
            //---------------------------------------------------

            leftButton = new UITextPanel<string>("◀");

            leftButton.Width.Set(40, 0);
            leftButton.Height.Set(40, 0);

            leftButton.Left.Set(210, 0);
            leftButton.Top.Set(650, 0);

            leftButton.OnLeftClick += PrevPage;

            Panel.Append(leftButton);

            //---------------------------------------------------
            // →

            rightButton = new UITextPanel<string>("▶");

            rightButton.Width.Set(40, 0);
            rightButton.Height.Set(40, 0);

            rightButton.Left.Set(350, 0);
            rightButton.Top.Set(650, 0);

            rightButton.OnLeftClick += NextPage;

            Panel.Append(rightButton);

            //---------------------------------------------------
            // ページ
            //---------------------------------------------------

            pageText = new UIText("");

            pageText.Left.Set(275, 0);
            pageText.Top.Set(660, 0);

            Panel.Append(pageText);

        }

        private void HireButtonClicked(
            UIMouseEvent evt,
            UIElement listeningElement)
        {

            MinerData data =
                MinerRosterSystem.Miners[currentPage];


            // 既に雇用済み
            if (data.IsHired)
                return;

            Player player = Main.LocalPlayer;

            // お金確認
            if (player.CountItem(ItemID.GoldCoin)<1)
            {
                Main.NewText(
                    "10ゴールド必要です。",
                    255, 100, 100
                );
                return;
            }


            player.BuyItem(10000);

            MinerData miner = MinerRosterSystem.Miners[currentPage];
            miner.ID = MinerRosterSystem.GetNextID();
            miner.Name = "新人鉱夫";
            miner.TexturePath = "MinerManagementMOD/Assets/UI/MinerPortrait";
            miner.IsHired = true;

            RefreshPage();


            Main.NewText(
                "新人鉱夫を雇いました！",
                100, 255, 100
            );
        }

        private void PrevPage(UIMouseEvent evt, UIElement listeningElement)
        {
            currentPage--;

            if (currentPage < 0)
                currentPage = MinerRosterSystem.MaxMiner - 1;

            RefreshPage();
        }

        private void NextPage(UIMouseEvent evt, UIElement listeningElement)
        {
            currentPage++;

            if (currentPage >= MinerRosterSystem.MaxMiner)
                currentPage = 0;

            RefreshPage();
        }

        private void SummonButtonClicked(UIMouseEvent evt, UIElement listeningElement)
        {

            MinerData data =
                MinerRosterSystem.Miners[currentPage];

            MinerManager.SpawnMiner(
                Main.LocalPlayer,
                data
            );
        }

        private void CloseWindow(UIMouseEvent evt, UIElement listeningElement)
        {
            MinerUISystem.Visible = false;
        }

        private void RefreshPage()
        {
            if (nameText == null || pageText == null)
                return;


            if (MinerRosterSystem.Miners == null)
                return;

            MinerData miner =
                MinerRosterSystem.Miners[currentPage];

            if (miner == null)
                return;

            nameText.SetText(miner.IsHired ? miner.Name : "空き");

            if (miner.IsHired)
            {
                if (hireButton.Parent != null)
                    hireButton.Remove();

                if (summonButton.Parent == null)
                    Panel.Append(summonButton);
            }
            else
            {
                if (summonButton.Parent != null)
                    summonButton.Remove();

                if (hireButton.Parent == null)
                    Panel.Append(hireButton);
            }

            pageText.SetText(
                $"{currentPage + 1}/{MinerRosterSystem.MaxMiner}"
                );
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            if (MinerRosterSystem.Miners == null)
                return;

            MinerData miner = MinerRosterSystem.Miners[currentPage];

            if (miner == null)
                return;

            if (string.IsNullOrEmpty(miner.TexturePath))
                return;

            Texture2D texture =
                ModContent.Request<Texture2D>(miner.TexturePath, AssetRequestMode.ImmediateLoad).Value;

            spriteBatch.Draw(
                texture,
                new Rectangle(
                    (int)(Panel.Left.Pixels + 20),
                    (int)(Panel.Top.Pixels + 80),
                    48,
                    48),
                Color.White);
        }
    }
}