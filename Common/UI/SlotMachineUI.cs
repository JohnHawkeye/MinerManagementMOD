using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using System;
using System.Collections.Generic;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using MinerManagementMOD.Items;
using Terraria.ModLoader.UI;

namespace MinerManagementMOD.UI
{
    public class SlotMachineUI : UIState
    {
        private UIPanel panel;

        // 5列 × 3行
        private const int Columns = 5;
        private const int Rows = 3;

        // 1マス32×32
        private const int CellSize = 32;

        // 絵柄
        private enum SlotSymbol
        {
            Pickaxe,
            Rope,
            Bomb,
            Ore,
            Ten,
            Jack,
            Queen,
            King,
            Spade,
            Heart,
            Club,
            Diamond,

            // ボーナス突入用シンボル
            Bonus
        }
        // --------------------------------
        // BET
        // --------------------------------

        private int betAmount = 1;

        private const int MinBet = 1;
        private const int MaxBet = 10;

        // 所持コイン表示
        private UIText coinText;

        // BET表示
        private UIText betText;

        // Bet変更ボタン
        private UIText decreaseBetButton;
        private UIText increaseBetButton;

        //autobutton
        private UIImageButton autoPlayButton;
        private bool isAutoPlay;

        private double autoPlayTimer;

        private const double AutoPlayDelay = 0.5;

        // Auto / Stop ボタン画像
        private Asset<Texture2D> autoTexture;
        private Asset<Texture2D> stopTexture;



        // --------------------------------
        // 当たり演出
        // --------------------------------
        private readonly Vector2[,] originalPositions =
            new Vector2[Columns, Rows];

        private List<SlotSymbol> winningSymbols =
            new List<SlotSymbol>();

        private int currentWinningIndex = 0;
        private double winEffectTimer = 0.0;

        // 1つの当たりを表示する時間
        private const double WinEffectDuration = 0.6;
        // 当たり演出中か
        private bool isShowingWinEffect;
        // 現在強調している絵柄
        private SlotSymbol? currentWinningSymbol = null;
        // 拡大率
        private float winScale = 1.0f;

        // 12種類の絵柄
        private readonly SlotSymbol[] symbols =
        {
            SlotSymbol.Pickaxe,
            SlotSymbol.Rope,
            SlotSymbol.Bomb,
            SlotSymbol.Ore,
            SlotSymbol.Ten,
            SlotSymbol.Jack,
            SlotSymbol.Queen,
            SlotSymbol.King,
            SlotSymbol.Spade,
            SlotSymbol.Heart,
            SlotSymbol.Club,
            SlotSymbol.Diamond
        };

        // 15マスの現在の結果
        private readonly SlotSymbol[,] result =
            new SlotSymbol[Columns, Rows];

        // 15個の画像
        private readonly UIImage[,] symbolImages =
            new UIImage[Columns, Rows];

        private readonly Random random = new Random();

        // --------------------------------
        // ボーナスゲーム
        // --------------------------------

        // 通常スピン1回ごとのボーナス突入抽選率
        // 1 / 100 = 1%
        private const double BonusTriggerChance = 1.0/60.0;

        // ボーナスゲームは5ラウンド
        private const int BonusRounds = 5;

        // 1ラウンドにつき3マスを崩す
        private const int BonusBreakCountPerRound = 3;

        // ボーナス終了後、通常モードへ戻るまでの待ち時間
        private const double BonusFinishDelay = 5.0;

        // ボーナス中にまだ残っている15マス
        private readonly bool[,] bonusBroken =
            new bool[Columns, Rows];

        // 崩したマスに表示する倍率
        private readonly string[,] bonusMultipliers =
            new string[Columns, Rows];

        // 倍率表示用UI
        private readonly UIText[,] bonusMultiplierTexts =
            new UIText[Columns, Rows];

        private bool isBonusGame;
        private bool isBonusIntro;
        private double bonusIntroTimer;
        private const double BonusIntroDuration = 3.0;
        private UIText bonusIntroText;
        private bool isBonusSpinning;

        private bool isBonusRevealing;
        private double bonusRevealTimer;
        private int bonusRevealIndex;
        private readonly List<int> bonusPendingPositions = new List<int>();
        private const double BonusRevealDuration = 0.45;
        private const double BonusRevealChangeTime = 0.18;

        // ボーナス開始時に15マスへ先に倍率を割り当てる
        private readonly string[,] bonusAssignedMultipliers =
            new string[Columns, Rows];
        private double bonusSpinTimer;
        private double bonusChangeTimer;
        private int bonusRound;
        private int bonusTotalMultiplier;
        private double bonusFinishTimer;
        private bool isShowingBonusResult;

        private const double BonusSpinDuration = 1.5;
        private const double BonusChangeInterval = 0.08;

        // 「ハズレ」の表示を含む5種類
        private readonly string[] bonusResults =
        {
            "ハズレ",
            "x1",
            "x2",
            "x5",
            "x10"
        };

        // 重み付き抽選。
        // ハズレが最も多く、x1/x2が次点、x5/x10は極めて低確率。
        private readonly int[] bonusResultWeights =
        {
            700, // ハズレ 70.0%
            200, // x1     20.0%
             80, // x2      8.0%
             18, // x5      1.8%
              2  // x10     0.2%
        };

        // --------------------------------
        // スロット演出用
        // --------------------------------

        private bool isSpinning;
        private double spinTimer;
        private const double SpinDuration = 2.0;
        private const double ChangeInterval = 0.05;
        private double changeTimer;
        private UIImageButton playButton;

        public override void OnInitialize()
        {
            panel = new UIPanel();

            panel.Width.Set(420f, 0f);
            panel.Height.Set(300f, 0f);

            panel.Left.Set(-210f, 0.5f);
            panel.Top.Set(-400f, 0.5f);

            Append(panel);

            CreateSlotCells();
            CreateBonusMultiplierTexts();
            CreateBonusIntroText();
            RandomizeSymbols();
            CreatePlayButton();
            CreateAutoPlayButton();

            CreateCoinDisplay();
            CreateBetControls();

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            double deltaTime =
                gameTime.ElapsedGameTime.TotalSeconds;

            // --------------------------------
            // ボーナスゲーム終了待ち
            // --------------------------------

            if (isShowingBonusResult)
            {
                bonusFinishTimer += deltaTime;

                if (bonusFinishTimer >= BonusFinishDelay)
                {
                    EndBonusGame();
                }

                return;
            }

            // --------------------------------
            // ボーナスゲーム中
            // --------------------------------

            if (isBonusIntro)
            {
                bonusIntroTimer += deltaTime;
                if (bonusIntroText != null)
                    bonusIntroText.SetText("ボーナスゲーム突入！！");

                if (bonusIntroTimer >= BonusIntroDuration)
                    BeginBonusBoard();

                return;
            }

            if (isBonusGame)
            {
                UpdateBonusGame(deltaTime);
                return;
            }

            // --------------------------------
            // 当たり演出中
            // --------------------------------

            if (isShowingWinEffect)
            {
                winEffectTimer += deltaTime;

                UpdateWinEffect();

                return;
            }

            // --------------------------------
            // スロット停止中
            // --------------------------------

            if (!isSpinning)
            {
                UpdateAutoPlay(deltaTime);
                return;
            }

            // --------------------------------
            // スロット回転中
            // --------------------------------

            spinTimer += deltaTime;
            changeTimer += deltaTime;

            if (changeTimer >= ChangeInterval)
            {
                changeTimer = 0.0;

                RandomizeSymbols();
            }

            if (spinTimer >= SpinDuration)
            {
                StopSpin();
            }
        }

        private void UpdateAutoPlay(double deltaTime)
        {
            if (!isAutoPlay)
                return;

            // 当たり演出中などは開始しない
            if (isShowingWinEffect ||
                isBonusGame ||
                isShowingBonusResult)
            {
                return;
            }

            autoPlayTimer -= deltaTime;

            if (autoPlayTimer <= 0.0)
            {
                autoPlayTimer = 0.0;

                StartSpin();

                // 次回までの待ち時間
                autoPlayTimer = AutoPlayDelay;
            }
        }

        private void CreateSlotCells()
        {
            for (int y = 0; y < Rows; y++)
            {
                for (int x = 0; x < Columns; x++)
                {
                    // 絵柄画像
                    UIImage image = new UIImage(
                        GetSymbolTexture(SlotSymbol.Pickaxe)
                    );

                    image.Width.Set(CellSize, 0f);
                    image.Height.Set(CellSize, 0f);

                    image.Left.Set(120f + x * CellSize, 0f);
                    image.Top.Set(60f + y * CellSize, 0f);

                    panel.Append(image);

                    symbolImages[x, y] = image;
                }
            }
        }

        private void CreateBonusMultiplierTexts()
        {
            for (int y = 0; y < Rows; y++)
            {
                for (int x = 0; x < Columns; x++)
                {
                    UIText text = new UIText("");
                    text.Width.Set(CellSize, 0f);
                    text.Height.Set(CellSize, 0f);
                    text.Left.Set(0f, 0f);
                    text.Top.Set(0f, 0f);

                    // 画像の上に倍率を表示するため、後からAppendする。
                    text.TextColor = Color.White;
                    text.HAlign = 0.5f;
                    text.VAlign = 0.5f;
                    text.OverflowHidden = false;

                    // 各UIImageの子要素にするので、文字が対応するBlock2の中に固定される。
                    symbolImages[x, y].Append(text);
                    bonusMultiplierTexts[x, y] = text;
                }
            }
        }

        private void CreateBonusIntroText()
        {
            bonusIntroText = new UIText("ボーナスゲーム突入！！", 1.5f);
            bonusIntroText.Width.Set(380f, 0f);
            bonusIntroText.Height.Set(50f, 0f);
            bonusIntroText.Left.Set(20f, 0f);
            bonusIntroText.Top.Set(32f, 0f);
            bonusIntroText.HAlign = 0.5f;
            bonusIntroText.VAlign = 0.5f;
            bonusIntroText.TextColor = Color.Gold;
            bonusIntroText.SetText("");
            panel.Append(bonusIntroText);
        }

        private void CreatePlayButton()
        {
            playButton = new UIImageButton(
                ModContent.Request<Texture2D>(
                    "MinerManagementMOD/Assets/UI/Slot/Play"
                )
            );

            playButton.Width.Set(32f, 0f);
            playButton.Height.Set(32f, 0f);

            // パネル中央
            playButton.Left.Set(194f, 0f);
            playButton.Top.Set(180f, 0f);

            playButton.OnLeftClick += (evt, element) =>
            {
                StartSpin();
            };

            panel.Append(playButton);
        }

        private void CreateAutoPlayButton()
        {
            autoTexture = ModContent.Request<Texture2D>(
                "MinerManagementMOD/Assets/UI/Slot/Auto"
            );

            stopTexture = ModContent.Request<Texture2D>(
                "MinerManagementMOD/Assets/UI/Slot/Stop"
            );

            autoPlayButton = new UIImageButton(autoTexture);

            autoPlayButton.Width.Set(32f, 0f);
            autoPlayButton.Height.Set(32f, 0f);

            // Playボタンの右横
            autoPlayButton.Left.Set(260f, 0f);
            autoPlayButton.Top.Set(180f, 0f);

            autoPlayButton.OnLeftClick += (evt, element) =>
            {
                ToggleAutoPlay();
            };

            panel.Append(autoPlayButton);
        }

        private void DisableAutoPlay()
        {
            isAutoPlay = false;
            autoPlayTimer = 0.0;

            if (autoPlayButton != null)
                autoPlayButton.SetImage(autoTexture);
        }

        private void ToggleAutoPlay()
        {
            isAutoPlay = !isAutoPlay;

            if (isAutoPlay)
            {
                autoPlayButton.SetImage(stopTexture);

                autoPlayTimer = AutoPlayDelay;
            }
            else
            {
                autoPlayButton.SetImage(autoTexture);

                autoPlayTimer = 0.0;
            }
        }

        private void PlaySpinSound()
        {
            SoundEngine.PlaySound(
                new SoundStyle("MinerManagementMOD/Assets/Sounds/Play"),
                Main.LocalPlayer.Center
            );
        }

        private void CreateCoinDisplay()
        {
            coinText = new UIText("");

            coinText.Left.Set(20f, 0f);
            coinText.Top.Set(225f, 0f);

            coinText.Width.Set(380f, 0f);
            coinText.Height.Set(24f, 0f);

            panel.Append(coinText);

            UpdateCoinDisplay();
        }

        private void UpdateCoinDisplay()
        {
            Player player = Main.LocalPlayer;

            int coinType =
                ModContent.ItemType<GoldMinerCoin>();

            int coinCount =
                player.CountItem(coinType);

            coinText.SetText(
                $"ゴールドマイナーコイン：{coinCount}"
            );
        }
        private void CreateBetControls()
        {
            // 左矢印
            decreaseBetButton = new UIText("◀");

            decreaseBetButton.Left.Set(145f, 0f);
            decreaseBetButton.Top.Set(255f, 0f);

            decreaseBetButton.Width.Set(32f, 0f);
            decreaseBetButton.Height.Set(32f, 0f);

            decreaseBetButton.OnLeftClick +=
                (evt, element) =>
                {
                    if (betAmount > MinBet)
                    {
                        betAmount--;
                        UpdateBetDisplay();
                    }
                };

            panel.Append(decreaseBetButton);


            // Bet表示
            betText = new UIText("");

            betText.Left.Set(180f, 0f);
            betText.Top.Set(255f, 0f);

            betText.Width.Set(60f, 0f);
            betText.Height.Set(32f, 0f);

            panel.Append(betText);


            // 右矢印
            increaseBetButton = new UIText("▶");

            increaseBetButton.Left.Set(245f, 0f);
            increaseBetButton.Top.Set(255f, 0f);

            increaseBetButton.Width.Set(32f, 0f);
            increaseBetButton.Height.Set(32f, 0f);

            increaseBetButton.OnLeftClick +=
                (evt, element) =>
                {
                    if (betAmount < MaxBet)
                    {
                        betAmount++;
                        UpdateBetDisplay();
                    }
                };

            panel.Append(increaseBetButton);

            UpdateBetDisplay();
        }
        private void UpdateBetDisplay()
        {
            betText.SetText(
                $"Bet {betAmount}"
            );
        }

        private void StartSpin()
        {
            // ボーナス中はコインを消費せず、ボーナススピンを開始
            if (isBonusGame)
            {
                PlaySpinSound();
                StartBonusSpin();
                return;
            }

            // すでに回っているなら何もしない
            if (isSpinning || isShowingWinEffect || isShowingBonusResult)
                return;

            Player player = Main.LocalPlayer;

            int goldMinerCoinType = ModContent.ItemType<GoldMinerCoin>();

            int ownedCoins = player.CountItem(goldMinerCoinType);

            // ゴールドコインを持っていなければ回せない
            if (ownedCoins < betAmount)
            {
                Main.NewText(
                    $"ゴールドマイナーコインがありません！ Bet:{betAmount}"
                );

                return;
            }

            // BET成功
            for (int i = 0; i < betAmount; i++)
            {
                player.ConsumeItem(goldMinerCoinType);
            }

            UpdateCoinDisplay();
            PlaySpinSound();

            isSpinning = true;

            spinTimer = 0.0;
            changeTimer = 0.0;

            // 最初の絵柄変更
            RandomizeSymbols();
        }

        private void StopSpin()
        {
            isSpinning = false;

            spinTimer = 0.0;
            changeTimer = 0.0;

            // 約100回に1回の確率でボーナス突入。
            // ボーナス発生時は3つのBonusシンボルを強制的に揃える。
            if (random.NextDouble() < BonusTriggerChance)
            {
                TriggerBonusSymbol();
                return;
            }

            // 最終結果を集計
            int[] counts = CountSymbols();

            // 当たりを取得
            winningSymbols.Clear();

            for (int i = 0; i < symbols.Length; i++)
            {
                SlotSymbol symbol = (SlotSymbol)i;

                int requiredCount = symbol switch
                {
                    // 小当たり：3個
                    SlotSymbol.Pickaxe => 3,
                    SlotSymbol.Rope => 3,
                    SlotSymbol.Bomb => 3,
                    SlotSymbol.Ore => 3,

                    // 大当たり：5個
                    SlotSymbol.Ten => 5,
                    SlotSymbol.Jack => 5,
                    SlotSymbol.Queen => 5,
                    SlotSymbol.King => 5,

                    // 中当たり：4個
                    SlotSymbol.Spade => 4,
                    SlotSymbol.Heart => 4,
                    SlotSymbol.Club => 4,
                    SlotSymbol.Diamond => 4,

                    _ => 999
                };

                if (counts[i] >= requiredCount)
                {
                    winningSymbols.Add((SlotSymbol)i);
                }
            }

            // ハズレ
            if (winningSymbols.Count == 0)
            {
                Main.NewText("ハズレ！");
                return;
            }

            // 当たり演出開始
            currentWinningIndex = 0;
            winEffectTimer = 0.0;
            isShowingWinEffect = true;

            StartCurrentWinEffect();
        }


        // ========================================
        // ボーナスゲーム
        // ========================================

        private void TriggerBonusSymbol()
        {

            DisableAutoPlay();

            // 通常の15マスをいったんランダムにする
            RandomizeSymbols();

            // 3か所をBonusシンボルに変更
            List<int> positions = new List<int>();

            while (positions.Count < 3)
            {
                int position = random.Next(Columns * Rows);

                if (!positions.Contains(position))
                {
                    positions.Add(position);
                }
            }

            foreach (int position in positions)
            {
                int x = position % Columns;
                int y = position / Columns;

                result[x, y] = SlotSymbol.Bonus;
                symbolImages[x, y].SetImage(GetSymbolTexture(SlotSymbol.Bonus));
            }

            Main.NewText("BONUS GAME!!");

            // BONUS絵柄3つ成立時の派手な「キーン！」
            SoundEngine.PlaySound(SoundID.ResearchComplete, Main.LocalPlayer.Center);
            SoundEngine.PlaySound(SoundID.Item4, Main.LocalPlayer.Center);

            // 3つ揃ったことを確認してからボーナスへ
            StartBonusGame();
        }

        private void StartBonusGame()
        {
            // まず通常スロット上で「ボーナスゲーム突入！！」を3秒表示
            isBonusGame = true;
            isBonusIntro = true;
            isBonusSpinning = false;
            isShowingBonusResult = false;
            bonusIntroTimer = 0.0;
            bonusRound = 0;
            bonusTotalMultiplier = 0;
            bonusSpinTimer = 0.0;
            bonusChangeTimer = 0.0;
            bonusFinishTimer = 0.0;

            Main.NewText("ボーナスゲーム突入！！");
            if (bonusIntroText != null)
                bonusIntroText.SetText("ボーナスゲーム突入！！");
        }

        private void BeginBonusBoard()
        {
            isBonusIntro = false;
            if (bonusIntroText != null)
                bonusIntroText.SetText("");

            // 15マスの当たりをここで先に決定
            for (int y = 0; y < Rows; y++)
            {
                for (int x = 0; x < Columns; x++)
                {
                    bonusAssignedMultipliers[x, y] = GetRandomBonusResult();
                    bonusBroken[x, y] = false;
                    bonusMultipliers[x, y] = "";

                    symbolImages[x, y].Width.Set(CellSize, 0f);
                    symbolImages[x, y].Height.Set(CellSize, 0f);
                    symbolImages[x, y].Left.Set(120f + x * CellSize, 0f);
                    symbolImages[x, y].Top.Set(60f + y * CellSize, 0f);
                    symbolImages[x, y].SetImage(GetBonusBlockTexture());

                    bonusMultiplierTexts[x, y].SetText("");
                    bonusMultiplierTexts[x, y].Width.Set(CellSize, 0f);
                    bonusMultiplierTexts[x, y].Height.Set(CellSize, 0f);
                    bonusMultiplierTexts[x, y].Left.Set(0f, 0f);
                    bonusMultiplierTexts[x, y].Top.Set(0f, 0f);
                }
            }

            Main.NewText("採掘ボーナス開始！");
        }

        private void UpdateBonusGame(double deltaTime)
        {
            if (isBonusSpinning)
            {
                bonusSpinTimer += deltaTime;
                bonusChangeTimer += deltaTime;

                if (bonusChangeTimer >= BonusChangeInterval)
                {
                    bonusChangeTimer = 0.0;
                    RandomizeBonusBlockDisplay();
                }

                if (bonusSpinTimer >= BonusSpinDuration)
                    StopBonusSpin();

                return;
            }

            if (isBonusRevealing)
                UpdateBonusReveal(deltaTime);
        }

        private void StartBonusSpin()
        {
            if (!isBonusGame ||
                isBonusIntro ||
                isBonusSpinning ||
                isBonusRevealing ||
                isShowingBonusResult ||
                bonusRound >= BonusRounds)
            {
                return;
            }

            // すでに15マスすべて崩れていたら終了
            if (CountUnbrokenBonusBlocks() < BonusBreakCountPerRound)
            {
                FinishBonusGame();
                return;
            }

            isBonusSpinning = true;
            bonusSpinTimer = 0.0;
            bonusChangeTimer = 0.0;

            RandomizeBonusBlockDisplay();
        }

        private void RandomizeBonusBlockDisplay()
        {
            // 残っているブロックだけ少しランダムに揺らす演出。
            // 実際に崩すのはStopBonusSpinで確定する。
            for (int y = 0; y < Rows; y++)
            {
                for (int x = 0; x < Columns; x++)
                {
                    if (!bonusBroken[x, y])
                    {
                        symbolImages[x, y].SetImage(
                            GetBonusBlockTexture()
                        );
                    }
                }
            }
        }

        private void StopBonusSpin()
        {
            isBonusSpinning = false;
            bonusSpinTimer = 0.0;
            bonusChangeTimer = 0.0;

            List<int> available = new List<int>();
            for (int y = 0; y < Rows; y++)
                for (int x = 0; x < Columns; x++)
                    if (!bonusBroken[x, y])
                        available.Add(y * Columns + x);

            bonusPendingPositions.Clear();
            for (int i = 0; i < BonusBreakCountPerRound; i++)
            {
                int index = random.Next(available.Count);
                bonusPendingPositions.Add(available[index]);
                available.RemoveAt(index);
            }

            bonusRevealIndex = 0;
            bonusRevealTimer = 0.0;
            isBonusRevealing = true;
        }

        private void UpdateBonusReveal(double deltaTime)
        {
            if (bonusRevealIndex >= bonusPendingPositions.Count)
            {
                isBonusRevealing = false;
                bonusRound++;

                if (bonusRound >= BonusRounds)
                    FinishBonusGame();
                else
                    Main.NewText($"ボーナス {bonusRound}/{BonusRounds}回目完了！");
                return;
            }

            int position = bonusPendingPositions[bonusRevealIndex];
            int x = position % Columns;
            int y = position / Columns;
            bonusRevealTimer += deltaTime;

            if (bonusRevealTimer < BonusRevealChangeTime)
            {
                float progress = (float)(bonusRevealTimer / BonusRevealChangeTime);
                SetBonusBlockScale(x, y, MathHelper.Lerp(1.0f, 1.18f, progress));
            }
            else if (bonusRevealTimer < BonusRevealDuration)
            {
                if (bonusRevealTimer - deltaTime < BonusRevealChangeTime)
                {
                    BreakBonusBlock(x, y);
                    SoundEngine.PlaySound(SoundID.Tink, Main.LocalPlayer.Center);
                }

                float progress = (float)((bonusRevealTimer - BonusRevealChangeTime) / (BonusRevealDuration - BonusRevealChangeTime));
                SetBonusBlockScale(x, y, MathHelper.Lerp(1.18f, 1.0f, progress));
            }
            else
            {
                SetBonusBlockScale(x, y, 1.0f);
                bonusRevealIndex++;
                bonusRevealTimer = 0.0;
            }
        }

        private void SetBonusBlockScale(int x, int y, float scale)
        {
            float size = CellSize * scale;
            float offset = (CellSize - size) / 2f;
            symbolImages[x, y].Width.Set(size, 0f);
            symbolImages[x, y].Height.Set(size, 0f);
            symbolImages[x, y].Left.Set(120f + x * CellSize + offset, 0f);
            symbolImages[x, y].Top.Set(60f + y * CellSize + offset, 0f);
        }

        private void BreakBonusBlock(int x, int y)
        {
            bonusBroken[x, y] = true;

            // ボーナス開始時に決めておいた結果を使用
            string resultText = bonusAssignedMultipliers[x, y];
            bonusMultipliers[x, y] = resultText;

            // 剥がした後はBlock2画像へ
            symbolImages[x, y].SetImage(GetBonusBlock2Texture());
            symbolImages[x, y].Width.Set(CellSize, 0f);
            symbolImages[x, y].Height.Set(CellSize, 0f);
            symbolImages[x, y].Left.Set(120f + x * CellSize, 0f);
            symbolImages[x, y].Top.Set(60f + y * CellSize, 0f);

            // ハズレは文字なし。それ以外は X1 / X2 / X5 / X10
            if (resultText == "ハズレ")
                bonusMultiplierTexts[x, y].SetText("");
            else
                bonusMultiplierTexts[x, y].SetText(resultText.ToUpperInvariant());

            if (resultText == "x1")
                bonusTotalMultiplier += 1;
            else if (resultText == "x2")
                bonusTotalMultiplier += 2;
            else if (resultText == "x5")
                bonusTotalMultiplier += 5;
            else if (resultText == "x10")
                bonusTotalMultiplier += 10;
        }

        private string GetRandomBonusResult()
        {
            int totalWeight = 0;

            foreach (int weight in bonusResultWeights)
                totalWeight += weight;

            int roll = random.Next(totalWeight);

            for (int i = 0; i < bonusResults.Length; i++)
            {
                if (roll < bonusResultWeights[i])
                    return bonusResults[i];

                roll -= bonusResultWeights[i];
            }

            return "ハズレ";
        }

        private int CountUnbrokenBonusBlocks()
        {
            int count = 0;

            for (int y = 0; y < Rows; y++)
            {
                for (int x = 0; x < Columns; x++)
                {
                    if (!bonusBroken[x, y])
                        count++;
                }
            }

            return count;
        }

        private void FinishBonusGame()
        {
            isBonusSpinning = false;
            isShowingBonusResult = true;
            bonusFinishTimer = 0.0;

            // 最終倍率表示
            Main.NewText(
                $"BONUS終了！ トータル倍率 x{bonusTotalMultiplier}"
            );

            // Bet × トータル倍率を払い戻す
            int payout = betAmount * bonusTotalMultiplier;

            if (payout > 0)
            {
                Player player = Main.LocalPlayer;

                int goldMinerCoinType =
                    ModContent.ItemType<GoldMinerCoin>();

                player.QuickSpawnItem(
                    player.GetSource_Misc("SlotMachineBonus"),
                    goldMinerCoinType,
                    payout
                );

                UpdateCoinDisplay();

                Main.NewText(
                    $"ゴールドマイナーコイン {payout}枚獲得！"
                );
            }
            else
            {
                Main.NewText("今回は払い戻しなし！");
            }

            Main.NewText("5秒後に通常スロットへ戻ります。");
        }

        private void EndBonusGame()
        {
            isBonusGame = false;
            isBonusIntro = false;
            if (bonusIntroText != null)
                bonusIntroText.SetText("");
            isBonusSpinning = false;
            isBonusRevealing = false;
            bonusIntroTimer = 0.0;
            bonusRevealTimer = 0.0;
            bonusRevealIndex = 0;
            bonusPendingPositions.Clear();
            isShowingBonusResult = false;

            bonusRound = 0;
            bonusTotalMultiplier = 0;
            bonusFinishTimer = 0.0;

            // 通常スロットの表示へ戻す
            ResetBonusImages();

            RandomizeSymbols();
        }

        private void ResetBonusImages()
        {
            for (int y = 0; y < Rows; y++)
            {
                for (int x = 0; x < Columns; x++)
                {
                    bonusBroken[x, y] = false;
                    bonusMultipliers[x, y] = "";

                    symbolImages[x, y].Width.Set(CellSize, 0f);
                    symbolImages[x, y].Height.Set(CellSize, 0f);
                    symbolImages[x, y].Left.Set(
                        120f + x * CellSize,
                        0f
                    );
                    symbolImages[x, y].Top.Set(
                        60f + y * CellSize,
                        0f
                    );

                    bonusMultiplierTexts[x, y].SetText("");
                    bonusMultiplierTexts[x, y].Left.Set(0f, 0f);
                    bonusMultiplierTexts[x, y].Top.Set(0f, 0f);
                    bonusMultiplierTexts[x, y].Width.Set(CellSize, 0f);
                    bonusMultiplierTexts[x, y].Height.Set(CellSize, 0f);
                }
            }
        }

        private Asset<Texture2D> GetBonusBlockTexture()
        {
            return ModContent.Request<Texture2D>(
                "MinerManagementMOD/Assets/UI/Slot/Block"
            );
        }

        private Asset<Texture2D> GetBonusBlock2Texture()
        {
            return ModContent.Request<Texture2D>(
                "MinerManagementMOD/Assets/UI/Slot/Block2"
            );
        }

        private void RandomizeSymbols()
        {
            for (int y = 0; y < Rows; y++)
            {
                for (int x = 0; x < Columns; x++)
                {
                    SlotSymbol symbol = GetRandomSymbol();

                    result[x, y] = symbol;

                    symbolImages[x, y].SetImage(
                        GetSymbolTexture(symbol)
                    );
                }
            }
        }

        private SlotSymbol GetRandomSymbol()
        {

            return random.Next(symbols.Length) switch
            {
                0 => SlotSymbol.Pickaxe,
                1 => SlotSymbol.Rope,
                2 => SlotSymbol.Bomb,
                3 => SlotSymbol.Ore,

                4 => SlotSymbol.Ten,
                5 => SlotSymbol.Jack,
                6 => SlotSymbol.Queen,
                7 => SlotSymbol.King,

                8 => SlotSymbol.Spade,
                9 => SlotSymbol.Heart,
                10 => SlotSymbol.Club,
                11 => SlotSymbol.Diamond,

                _ => SlotSymbol.Pickaxe
            };
        }

        private int[] CountSymbols()
        {
            int[] counts = new int[symbols.Length];

            for (int y = 0; y < Rows; y++)
            {
                for (int x = 0; x < Columns; x++)
                {
                    counts[(int)result[x, y]]++;
                }
            }

            return counts;
        }

        private void StartCurrentWinEffect()
        {
            if (currentWinningIndex >= winningSymbols.Count)
            {
                FinishWinEffect();
                return;
            }

            currentWinningSymbol =
                winningSymbols[currentWinningIndex];

            winScale = 1.0f;

            // 調査完了の「ピラリン♪」
            SoundEngine.PlaySound(
                SoundID.ResearchComplete
            );
        }

        private void UpdateWinEffect()
        {
            if (currentWinningSymbol == null)
                return;

            float scale;

            // 拡大
            if (winEffectTimer < 0.15)
            {
                float progress =
                    (float)(winEffectTimer / 0.15);

                scale = MathHelper.Lerp(
                    1.0f,
                    1.2f,
                    progress
                );
            }
            else
            {
                // 少し戻す
                float progress =
                    (float)((winEffectTimer - 0.15) / 0.45);

                progress = MathHelper.Clamp(
                    progress,
                    0f,
                    1f
                );

                scale = MathHelper.Lerp(
                    1.2f,
                    1.05f,
                    progress
                );
            }

            winScale = scale;

            // ==========================================
            // 現在の当たり絵柄を中央から拡大
            // ==========================================

            for (int y = 0; y < Rows; y++)
            {
                for (int x = 0; x < Columns; x++)
                {
                    if (result[x, y] != currentWinningSymbol.Value)
                        continue;

                    UIImage image = symbolImages[x, y];

                    float size = CellSize * winScale;

                    // 元の画像位置
                    float baseX = 120f + x * CellSize;
                    float baseY = 60f + y * CellSize;

                    // 32×32の中心を基準に拡大
                    float offset =
                        (CellSize - size) / 2f;

                    image.Width.Set(size, 0f);
                    image.Height.Set(size, 0f);

                    image.Left.Set(
                        baseX + offset,
                        0f
                    );

                    image.Top.Set(
                        baseY + offset,
                        0f
                    );
                }
            }

            // ==========================================
            // 演出終了
            // ==========================================

            if (winEffectTimer >= WinEffectDuration)
            {
                ResetWinningImages();

                currentWinningIndex++;
                winEffectTimer = 0.0;

                if (currentWinningIndex >= winningSymbols.Count)
                {
                    FinishWinEffect();
                }
                else
                {
                    StartCurrentWinEffect();
                }
            }
        }

        private void ResetWinningImages()
        {
            for (int y = 0; y < Rows; y++)
            {
                for (int x = 0; x < Columns; x++)
                {
                    UIImage image = symbolImages[x, y];

                    image.Width.Set(CellSize, 0f);
                    image.Height.Set(CellSize, 0f);

                    // 本来の位置へ戻す
                    image.Left.Set(
                        120f + x * CellSize,
                        0f
                    );

                    image.Top.Set(
                        60f + y * CellSize,
                        0f
                    );
                }
            }

            winScale = 1.0f;
        }

        private void FinishWinEffect()
        {
            isShowingWinEffect = false;

            currentWinningSymbol = null;
            winScale = 1.0f;

            // 配当計算
            int payout = 0;

            foreach (SlotSymbol symbol in winningSymbols)
            {
                payout += GetPayoutMultiplier(symbol) * betAmount;
            }

            if (payout <= 0)
            {
                return;
            }

            Player player = Main.LocalPlayer;

            int goldMinerCoinType =
                ModContent.ItemType<GoldMinerCoin>();

            player.QuickSpawnItem(
                player.GetSource_Misc("SlotMachine"),
                goldMinerCoinType,
                payout
            );

            Main.NewText(
                $"ゴールドマイナーコイン {payout}枚獲得！"
            );
        }

        private int GetPayoutMultiplier(SlotSymbol symbol)
        {
            // 小当たり
            if (symbol >= SlotSymbol.Pickaxe &&
                symbol <= SlotSymbol.Ore)
            {
                return 1;
            }

            // 中当たり
            if (symbol >= SlotSymbol.Spade &&
                symbol <= SlotSymbol.Diamond)
            {
                return 3;
            }

            // 大当たり
            if (symbol >= SlotSymbol.Ten &&
                symbol <= SlotSymbol.King)
            {
                return 10;
            }

            return 0;
        }

        private int GetRequiredCount(SlotSymbol symbol)
        {
            if (symbol >= SlotSymbol.Pickaxe &&
                symbol <= SlotSymbol.Ore)
            {
                return 3;
            }

            if (symbol >= SlotSymbol.Spade &&
                symbol <= SlotSymbol.Diamond)
            {
                return 4;
            }

            if (symbol >= SlotSymbol.Ten &&
                symbol <= SlotSymbol.King)
            {
                return 5;
            }

            return 0;
        }

        private Asset<Texture2D> GetSymbolTexture(
            SlotSymbol symbol)
        {
            string path = symbol switch
            {
                SlotSymbol.Pickaxe =>
                    "MinerManagementMOD/Assets/UI/Slot/Pickaxe",

                SlotSymbol.Rope =>
                    "MinerManagementMOD/Assets/UI/Slot/Rope",

                SlotSymbol.Bomb =>
                    "MinerManagementMOD/Assets/UI/Slot/Bomb",

                SlotSymbol.Ore =>
                    "MinerManagementMOD/Assets/UI/Slot/Ore",

                SlotSymbol.Ten =>
                    "MinerManagementMOD/Assets/UI/Slot/Ten",

                SlotSymbol.Jack =>
                    "MinerManagementMOD/Assets/UI/Slot/Jack",

                SlotSymbol.Queen =>
                    "MinerManagementMOD/Assets/UI/Slot/Queen",

                SlotSymbol.King =>
                    "MinerManagementMOD/Assets/UI/Slot/King",

                SlotSymbol.Spade =>
                    "MinerManagementMOD/Assets/UI/Slot/Spade",

                SlotSymbol.Heart =>
                    "MinerManagementMOD/Assets/UI/Slot/Heart",

                SlotSymbol.Club =>
                    "MinerManagementMOD/Assets/UI/Slot/Club",

                SlotSymbol.Diamond =>
                    "MinerManagementMOD/Assets/UI/Slot/Diamond",

                SlotSymbol.Bonus =>
                    "MinerManagementMOD/Assets/UI/Slot/Bonus",

                _ =>
                    "MinerManagementMOD/Assets/UI/Slot/Pickaxe"
            };

            return ModContent.Request<Texture2D>(path);
        }
    }
}