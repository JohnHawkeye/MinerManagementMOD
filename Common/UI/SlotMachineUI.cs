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
            Diamond
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
            RandomizeSymbols();
            CreatePlayButton();

            CreateCoinDisplay();
            CreateBetControls();

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            double deltaTime =
                gameTime.ElapsedGameTime.TotalSeconds;

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

        private void CreateSlotCells()
        {
            for (int y = 0; y < Rows; y++)
            {
                for (int x = 0; x < Columns; x++)
                {
                    // 1マスの背景
                    UIPanel cell = new UIPanel();

                    cell.Width.Set(CellSize, 0f);
                    cell.Height.Set(CellSize, 0f);

                    cell.Left.Set(
                        120f + x * CellSize,
                        0f
                    );

                    cell.Top.Set(
                        60f + y * CellSize,
                        0f
                    );

                    panel.Append(cell);

                    // 絵柄画像
                    UIImage image = new UIImage(
                        GetSymbolTexture(SlotSymbol.Pickaxe)
                    );

                    image.Width.Set(CellSize, 0f);
                    image.Height.Set(CellSize, 0f);

                    image.Left.Set(0f, 0f);
                    image.Top.Set(0f, 0f);

                    cell.Append(image);

                    symbolImages[x, y] = image;
                }
            }
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
            // すでに回っているなら何もしない
            if (isSpinning || isShowingWinEffect)
                return;

            Player player = Main.LocalPlayer;

            int goldMinerCoinType = ModContent.ItemType<GoldMinerCoin>();

            int ownedCoins = player.CountItem(goldMinerCoinType);

            // ゴールドコインを持っていなければ回せない
            if (ownedCoins <betAmount)
            {
                Main.NewText(
                    $"ゴールドマイナーコインがありません！ Bet:{betAmount}"
                );

                return;
            }

            // BET成功
            for(int i = 0;i <betAmount; i++)
            {
                player.ConsumeItem(goldMinerCoinType);
            }

            UpdateCoinDisplay();

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

            // 最終結果を集計
            int[] counts = CountSymbols();

            // 当たりを取得
            winningSymbols.Clear();

            // 小当たり
            for (int i = (int)SlotSymbol.Pickaxe;
                 i <= (int)SlotSymbol.Ore;
                 i++)
            {
                if (counts[i] >= 3)
                {
                    winningSymbols.Add((SlotSymbol)i);
                }
            }

            // 中当たり
            for (int i = (int)SlotSymbol.Spade;
                 i <= (int)SlotSymbol.Diamond;
                 i++)
            {
                if (counts[i] >= 3)
                {
                    winningSymbols.Add((SlotSymbol)i);
                }
            }

            // 大当たり
            for (int i = (int)SlotSymbol.Ten;
                 i <= (int)SlotSymbol.King;
                 i++)
            {
                if (counts[i] >= 3)
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
            // 出現率
            const int SmallWeight = 50;
            const int MediumWeight = 30;
            const int BigWeight = 20;

            int totalWeight =
                SmallWeight +
                MediumWeight +
                BigWeight;

            int value = random.Next(totalWeight);

            // 小当たり
            if (value < SmallWeight)
            {
                return GetRandomSmallSymbol();
            }

            value -= SmallWeight;

            // 中当たり
            if (value < MediumWeight)
            {
                return GetRandomMediumSymbol();
            }

            // 大当たり
            return GetRandomBigSymbol();
        }
        private SlotSymbol GetRandomSmallSymbol()
        {
            return random.Next(4) switch
            {
                0 => SlotSymbol.Pickaxe,
                1 => SlotSymbol.Rope,
                2 => SlotSymbol.Bomb,
                _ => SlotSymbol.Ore
            };
        }

        private SlotSymbol GetRandomMediumSymbol()
        {
            return random.Next(4) switch
            {
                0 => SlotSymbol.Spade,
                1 => SlotSymbol.Heart,
                2 => SlotSymbol.Club,
                _ => SlotSymbol.Diamond
            };
        }

        private SlotSymbol GetRandomBigSymbol()
        {
            return random.Next(4) switch
            {
                0 => SlotSymbol.Ten,
                1 => SlotSymbol.Jack,
                2 => SlotSymbol.Queen,
                _ => SlotSymbol.King
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

        private void CheckResult(int[] counts)
        {
            List<SlotSymbol> winningSymbols =
                new List<SlotSymbol>();

            // 小当たり
            for (int i = (int)SlotSymbol.Pickaxe;
                 i <= (int)SlotSymbol.Ore;
                 i++)
            {
                if (counts[i] >= 3)
                {
                    winningSymbols.Add((SlotSymbol)i);
                }
            }

            // 中当たり
            for (int i = (int)SlotSymbol.Spade;
                 i <= (int)SlotSymbol.Diamond;
                 i++)
            {
                if (counts[i] >= 3)
                {
                    winningSymbols.Add((SlotSymbol)i);
                }
            }

            // 大当たり
            for (int i = (int)SlotSymbol.Ten;
                 i <= (int)SlotSymbol.King;
                 i++)
            {
                if (counts[i] >= 3)
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

            // 配当計算
            int payout = 0;

            foreach (SlotSymbol symbol in winningSymbols)
            {
                int multiplier = GetPayoutMultiplier(symbol);

                payout += multiplier;
            }

            // ゴールドコインを払い出す
            Player player = Main.LocalPlayer;

            if (payout > 0)
            {
                player.QuickSpawnItem(
                    player.GetSource_Misc("SlotMachine"),
                    ModContent.ItemType<GoldMinerCoin>(),
                    payout
                );
            }

            // 結果表示
            Main.NewText(
                $"当たり！ ゴールドマイナーコイン {payout}枚獲得！"
            );

            // それぞれの当たりも表示
            foreach (SlotSymbol symbol in winningSymbols)
            {
                Main.NewText(
                    $"{symbol} ×3 → {GetPayoutMultiplier(symbol)}枚"
                );
            }
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

            // 現在の当たり絵柄を拡大
            for (int y = 0; y < Rows; y++)
            {
                for (int x = 0; x < Columns; x++)
                {
                    if (result[x, y] != currentWinningSymbol.Value)
                        continue;

                    UIImage image = symbolImages[x, y];

                    float size = CellSize * winScale;

                    // サイズ変更
                    image.Width.Set(size, 0f);
                    image.Height.Set(size, 0f);

                    // 中心を32×32マスの中央に維持
                    float offset =
                        (CellSize - size) / 2f;

                    image.Left.Set(offset, 0f);
                    image.Top.Set(offset, 0f);
                }
            }

            // 演出終了
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

                    image.Left.Set(0f, 0f);
                    image.Top.Set(0f, 0f);
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
                return 2;
            }

            // 中当たり
            if (symbol >= SlotSymbol.Spade &&
                symbol <= SlotSymbol.Diamond)
            {
                return 5;
            }

            // 大当たり
            if (symbol >= SlotSymbol.Ten &&
                symbol <= SlotSymbol.King)
            {
                return 15;
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

                _ =>
                    "MinerManagementMOD/Assets/UI/Slot/Pickaxe"
            };

            return ModContent.Request<Texture2D>(path);
        }
    }
}