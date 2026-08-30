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
using MinerManagementMOD.NPCs;
using Terraria.Audio;
using MinerManagementMOD.Items;

namespace MinerManagementMOD.Common.UI
{
    public class MinerRosterUI : UIState
    {
        public UIPanel Panel;

        private UIText nameText;
        private UIText pageText;
        private UIText miningLevelText;
        private UIText styleInfoText;

        private UITextPanel<string> summonButton;
        private UITextPanel<string> gotoButton;
        private UITextPanel<string> lightButton;
        private UITextPanel<string> levelUpButton;

        private UITextPanel<string> leftButton;
        private UITextPanel<string> rightButton;
        private UITextPanel<string> closeButton;
        private UITextPanel<string> hireButton;

        private UIImage portraitImage;

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

            //Panel.BackgroundColor = new Color(63,82,151);
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
            // ポートレイト画像
            //---------------------------------------------------


            portraitImage = new UIImage(
            ModContent.Request<Texture2D>(
                "MinerManagementMOD/Assets/UI/MinerPortrait"));

            portraitImage.Left.Set(20, 0);
            portraitImage.Top.Set(80, 0);

            portraitImage.Width.Set(48, 0);
            portraitImage.Height.Set(48, 0);

            UIPanel portraitPanel = new UIPanel();
            portraitPanel.Left.Set(10, 0);
            portraitPanel.Top.Set(70, 0);
            portraitPanel.Width.Set(56, 0);
            portraitPanel.Height.Set(56, 0);

            portraitPanel.Append(portraitImage);

            Panel.Append(portraitImage);


            //---------------------------------------------------
            // 名前
            //---------------------------------------------------

            nameText = new UIText("");

            nameText.Left.Set(90, 0);
            nameText.Top.Set(95, 0);

            Panel.Append(nameText);

            //mininglevel
            miningLevelText = new UIText("");

            miningLevelText.Left.Set(90, 0);
            miningLevelText.Top.Set(120, 0);

            Panel.Append(miningLevelText);

            //---------------------------------------------------
            // 特徴説明
            //---------------------------------------------------

            styleInfoText = new UIText("");

            styleInfoText.Left.Set(90, 0);
            styleInfoText.Top.Set(200, 0);

            styleInfoText.Width.Set(480, 0);

            Panel.Append(styleInfoText);

            //-----------------------------------------------------
            //summon button
            //-----------------------------------------------------
            summonButton = new UITextPanel<string>("召喚");
            summonButton.Left.Set(260, 0);
            summonButton.Top.Set(80, 0);
            summonButton.OnLeftClick += SummonButtonClicked;
            Panel.Append(summonButton);

            //-----------------------------------------------------
            // Goto button
            //-----------------------------------------------------
            gotoButton = new UITextPanel<string>("傍に行く");

            gotoButton.Width.Set(120, 0);
            gotoButton.Height.Set(40, 0);

            gotoButton.Left.Set(390, 0);   // 召喚ボタンの右
            gotoButton.Top.Set(80, 0);

            gotoButton.OnLeftClick += GotoButtonClicked;

            Panel.Append(gotoButton);

            //-----------------------------------------------------
            // Light button
            //-----------------------------------------------------
            lightButton = new UITextPanel<string>("照明を装備\n1G");

            lightButton.Width.Set(120, 0);
            lightButton.Height.Set(50, 0);

            lightButton.Left.Set(260, 0);
            lightButton.Top.Set(140, 0);

            lightButton.OnLeftClick += LightButtonClicked;

            Panel.Append(lightButton);

            //mining level up
            levelUpButton = new UITextPanel<string>("レベルアップ\n1 Platinum");

            levelUpButton.Width.Set(120, 0);
            levelUpButton.Height.Set(50, 0);

            levelUpButton.Left.Set(390, 0);
            levelUpButton.Top.Set(140, 0);

            levelUpButton.OnLeftClick += LevelUpButtonClicked;

            Panel.Append(levelUpButton);

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

            // 3番目の鉱夫（MagicMiner）は
            // プラチナマイナーコイン10枚
            if (currentPage == 2)
            {
                int platinumMinerCoin =
                    ModContent.ItemType<PlatinumMinerCoin>();

                if (player.CountItem(platinumMinerCoin) < 10)
                {
                    Main.NewText(
                        "プラチナマイナーコインが10枚必要です。",
                        255, 100, 100
                    );
                    return;
                }

                for (int i = 0; i < 10; i++)
                {
                    player.ConsumeItem(platinumMinerCoin);
                }
            }// 5人目：採掘の女神
            else if (currentPage == 4)
            {
                int platinumMinerCoin =
                    ModContent.ItemType<PlatinumMinerCoin>();

                if (player.CountItem(platinumMinerCoin) < 100)
                {
                    Main.NewText(
                        "採掘の女神を雇うには、プラチナマイナーコインが100枚必要です。",
                        255, 100, 100
                    );
                    return;
                }

                for (int i = 0; i < 100; i++)
                {
                    player.ConsumeItem(platinumMinerCoin);
                }
            }
            // それ以外の鉱夫は
            // ゴールドマイナーコイン10枚
            else
            {
                int goldMinerCoin =
                    ModContent.ItemType<GoldMinerCoin>();

                if (player.CountItem(goldMinerCoin) < 10)
                {
                    Main.NewText(
                        "ゴールドマイナーコインが10枚必要です。",
                        255, 100, 100
                    );
                    return;
                }

                for (int i = 0; i < 10; i++)
                {
                    player.ConsumeItem(goldMinerCoin);
                }
            }

            // 雇用処理
            MinerData miner = MinerRosterSystem.Miners[currentPage];

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

            MinerManager.SpawnMiner(Main.LocalPlayer, data);

            RefreshPage();
        }

        private void GotoButtonClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            MinerData miner = MinerRosterSystem.Miners[currentPage];

            foreach (NPC npc in Main.npc)
            {
                if (!npc.active)
                    continue;

                if (npc.ModNPC is MinerNPC minerNpc &&
                    minerNpc.MinerID == miner.ID)
                {
                    // NPCの少し横へワープ
                    SoundEngine.PlaySound(SoundID.Item6, Main.LocalPlayer.Center);
                    Main.LocalPlayer.Teleport(
                        npc.Center + new Vector2(0f, -24f),
                        TeleportationStyleID.RodOfDiscord
                    );
                    SoundEngine.PlaySound(SoundID.Item6, Main.LocalPlayer.Center);
                    break;
                }
            }
        }

        private void LightButtonClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            MinerData miner = MinerRosterSystem.Miners[currentPage];

            if (!miner.IsHired)
                return;

            if (miner.HasLight)
            {
                Main.NewText("この鉱夫は既に照明を装備しています。");
                return;
            }

            Player player = Main.LocalPlayer;

            // 1Gold必要
            if (!player.BuyItem(Item.buyPrice(gold: 1)))
            {
                Main.NewText("1ゴールド必要です。", 255, 100, 100);
                return;
            }

            miner.HasLight = true;

            Main.NewText($"{miner.Name}に照明を装備させました！", 100, 255, 100);

            RefreshPage();
        }

        private void LevelUpButtonClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            MinerData miner = MinerRosterSystem.Miners[currentPage];

            if (!miner.IsHired)
                return;

            Player player = Main.LocalPlayer;

            int platinumMinerCoin =
                ModContent.ItemType<PlatinumMinerCoin>();

            if (player.CountItem(platinumMinerCoin) < 1)
            {
                Main.NewText("プラチナマイナーコインが必要です。", 255, 100, 100);
                return;
            }

            player.ConsumeItem(platinumMinerCoin);

            miner.MiningLevel++;

            Main.NewText(
                $"{miner.Name}の採掘レベルが{miner.MiningLevel}になりました！",
                100, 255, 100);

            RefreshPage();
        }

        private void CloseWindow(UIMouseEvent evt, UIElement listeningElement)
        {
            MinerUISystem.Visible = false;
        }

        public void RefreshPage()
        {
            if (nameText == null || pageText == null)
                return;


            if (MinerRosterSystem.Miners == null)
                return;

            MinerData miner =
                MinerRosterSystem.Miners[currentPage];

            // 雇用料金表示
            if (!miner.IsHired)
            {
                if (currentPage == 2)
                {
                    hireButton.SetText("雇う\n10P");
                }
                else if (currentPage == 4)
                {
                    hireButton.SetText("雇う\n100P");
                }
                else
                {
                    hireButton.SetText("雇う\n10G");
                }
            }

            //Summon button
            if (miner.IsHired)
            {
                if (MinerManager.IsMinerSpawned(miner.ID))
                {
                    summonButton.SetText("召喚中");
                    summonButton.TextColor = Color.LimeGreen;

                    if (gotoButton.Parent == null)
                        Panel.Append(gotoButton);
                }
                else
                {
                    summonButton.SetText("召喚");
                    summonButton.TextColor = Color.White;

                    if (gotoButton.Parent != null)
                        gotoButton.Remove();
                }
            }

            //Light 
            if (miner.HasLight)
            {
                lightButton.SetText("照明装備済");
                lightButton.TextColor = Color.Yellow;
            }
            else
            {
                lightButton.SetText("照明を装備\n1G");
                lightButton.TextColor = Color.White;
            }

            //mining level
            miningLevelText.SetText($"採掘レベル : {miner.MiningLevel}");

            // 特徴説明
            styleInfoText.SetText(miner.StyleInfo ?? "");

            if (miner == null)
                return;

            if (!string.IsNullOrEmpty(miner.TexturePath))
            {
                portraitImage.SetImage(
                    ModContent.Request<Texture2D>(
                        miner.TexturePath,
                        AssetRequestMode.ImmediateLoad));
            }

            nameText.SetText(miner.Name);

            if (miner.IsHired)
            {
                if (hireButton.Parent != null)
                    hireButton.Remove();

                if (summonButton.Parent == null)
                    Panel.Append(summonButton);

                if (lightButton.Parent == null)
                    Panel.Append(lightButton);

                if (levelUpButton.Parent == null)
                    Panel.Append(levelUpButton);
            }
            else
            {
                if (summonButton.Parent != null)
                    summonButton.Remove();

                if (gotoButton.Parent != null)
                    gotoButton.Remove();

                if (lightButton.Parent != null)
                    lightButton.Remove();

                if (levelUpButton.Parent != null)
                    levelUpButton.Remove();

                if (hireButton.Parent == null)
                    Panel.Append(hireButton);
            }

            pageText.SetText(
                $"{currentPage + 1}/{MinerRosterSystem.MaxMiner}"
                );


            // miner = MinerRosterSystem.Miners[currentPage];

            // Main.NewText(
            //     $"ID={miner.ID} / " +
            //     $"Name={miner.Name} / " +
            //     $"IsHired={miner.IsHired} / " +
            //     $"Level={miner.MiningLevel} / " +
            //     $"Power={miner.MiningPower} / " +
            //     $"Speed={miner.MiningSpeed} / " +
            //     $"Capacity={miner.CarryCapacity} / " +
            //     $"Bonus={miner.OreBonusChance} / " +
            //     $"Light={miner.HasLight}"
            // );
        }

    }
}