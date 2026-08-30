using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MinerManagementMOD.Systems
{
    public class TradeExchangeSystem : ModSystem
    {
        // =========================================================
        // 交換表
        // =========================================================

        private static readonly Dictionary<int, int> ExchangeTable =
            new Dictionary<int, int>
            {
                // =================================================
                // 鉱石
                // =================================================

                { ItemID.CopperOre, ItemID.TinOre },
                { ItemID.TinOre, ItemID.CopperOre },

                { ItemID.IronOre, ItemID.LeadOre },
                { ItemID.LeadOre, ItemID.IronOre },

                { ItemID.SilverOre, ItemID.TungstenOre },
                { ItemID.TungstenOre, ItemID.SilverOre },

                { ItemID.GoldOre, ItemID.PlatinumOre },
                { ItemID.PlatinumOre, ItemID.GoldOre },

                { ItemID.DemoniteOre, ItemID.CrimtaneOre },
                { ItemID.CrimtaneOre, ItemID.DemoniteOre },

                { ItemID.CobaltOre, ItemID.PalladiumOre },
                { ItemID.PalladiumOre, ItemID.CobaltOre },

                { ItemID.MythrilOre, ItemID.OrichalcumOre },
                { ItemID.OrichalcumOre, ItemID.MythrilOre },

                { ItemID.AdamantiteOre, ItemID.TitaniumOre },
                { ItemID.TitaniumOre, ItemID.AdamantiteOre },


                // =================================================
                // バー
                // =================================================

                { ItemID.CopperBar, ItemID.TinBar },
                { ItemID.TinBar, ItemID.CopperBar },

                { ItemID.IronBar, ItemID.LeadBar },
                { ItemID.LeadBar, ItemID.IronBar },

                { ItemID.SilverBar, ItemID.TungstenBar },
                { ItemID.TungstenBar, ItemID.SilverBar },

                { ItemID.GoldBar, ItemID.PlatinumBar },
                { ItemID.PlatinumBar, ItemID.GoldBar },

                { ItemID.DemoniteBar, ItemID.CrimtaneBar },
                { ItemID.CrimtaneBar, ItemID.DemoniteBar },

                { ItemID.CobaltBar, ItemID.PalladiumBar },
                { ItemID.PalladiumBar, ItemID.CobaltBar },

                { ItemID.MythrilBar, ItemID.OrichalcumBar },
                { ItemID.OrichalcumBar, ItemID.MythrilBar },

                { ItemID.AdamantiteBar, ItemID.TitaniumBar },
                { ItemID.TitaniumBar, ItemID.AdamantiteBar }
            };


        // =========================================================
        // 左クリック状態
        // =========================================================

        private bool previousMouseLeft = false;

        // 一度交換したら、マウスを離すまで交換しない
        private bool exchangeClickLock = false;


        // =========================================================
        // ワールド開始
        // =========================================================

        public override void OnWorldLoad()
        {
            previousMouseLeft = false;
            exchangeClickLock = false;
        }


        // =========================================================
        // ワールド終了
        // =========================================================

        public override void OnWorldUnload()
        {
            previousMouseLeft = false;
            exchangeClickLock = false;
        }


        // =========================================================
        // 入力処理
        // =========================================================

        public override void PostUpdateInput()
        {
            Player player = Main.LocalPlayer;

            if (player == null || !player.active)
                return;


            // -----------------------------------------------------
            // マウスを離したらロック解除
            // -----------------------------------------------------

            if (!Main.mouseLeft)
            {
                exchangeClickLock = false;
            }


            // -----------------------------------------------------
            // 左クリックを押した瞬間
            // -----------------------------------------------------

            bool clickedThisFrame =
                Main.mouseLeft &&
                !previousMouseLeft;


            // 次フレーム用に保存
            previousMouseLeft = Main.mouseLeft;


            if (!clickedThisFrame)
                return;


            // -----------------------------------------------------
            // 既に今回のクリックで交換済みなら終了
            // -----------------------------------------------------

            if (exchangeClickLock)
                return;


            // -----------------------------------------------------
            // カーソルにアイテムがない
            // -----------------------------------------------------

            if (Main.mouseItem == null ||
                Main.mouseItem.IsAir)
            {
                return;
            }


            // -----------------------------------------------------
            // マウス位置
            // -----------------------------------------------------

            Vector2 mouseWorld = Main.MouseWorld;

            int tileX =
                (int)(mouseWorld.X / 16f);

            int tileY =
                (int)(mouseWorld.Y / 16f);


            if (!WorldGen.InWorld(tileX, tileY, 1))
                return;


            Tile tile =
                Main.tile[tileX, tileY];


            if (tile == null ||
                !tile.HasTile)
            {
                return;
            }


            // -----------------------------------------------------
            // 交換機か確認
            // -----------------------------------------------------

            int exchangeMachineType =
                ModContent.TileType<
                    Tiles.TradeExchangeMachineTile>();


            if (tile.TileType != exchangeMachineType)
                return;


            // -----------------------------------------------------
            // 交換実行
            // -----------------------------------------------------

            bool exchanged =
                TryExchange(
                    player,
                    tileX,
                    tileY);


            // -----------------------------------------------------
            // 成功した場合はクリックをロック
            // -----------------------------------------------------

            if (exchanged)
            {
                exchangeClickLock = true;
            }
        }


        // =========================================================
        // 交換処理
        // =========================================================

        private bool TryExchange(
    Player player,
    int tileX,
    int tileY)
        {
            Item input = Main.mouseItem;

            // ==========================================
            // 入力アイテム確認
            // ==========================================

            if (input == null ||
                input.IsAir ||
                input.stack <= 0)
            {
                return false;
            }


            // ==========================================
            // 交換対象か確認
            // ==========================================

            if (!ExchangeTable.TryGetValue(
                    input.type,
                    out int outputType))
            {
                return false;
            }


            // ==========================================
            // 交換数量
            // ==========================================

            int exchangeAmount = input.stack;

            if (exchangeAmount <= 0)
                return false;


            // ==========================================
            // カッパーマイナーコイン
            // ==========================================

            int coinType =
                ModContent.ItemType<
                    Items.CopperMinerCoin>();


            // ==========================================
            // コインの総数を確認
            // ==========================================

            int coinCount =
                CountCopperMinerCoins(
                    player,
                    coinType);


            // ==========================================
            // コイン不足なら交換しない
            // ==========================================

            if (coinCount < exchangeAmount)
            {
                Main.NewText(
                    $"カッパーマイナーコインが足りないよ。" +
                    $" 必要数: {exchangeAmount}枚 / 所持数: {coinCount}枚",
                    Color.Orange);

                return false;
            }


            // ==========================================
            // 交換機の左上座標を取得
            // ==========================================

            GetMachineTopLeft(
                tileX,
                tileY,
                out int topLeftX,
                out int topLeftY);


            // ==========================================
            // 交換機の上から出す位置
            // ==========================================

            Vector2 dropPosition =
                new Vector2(
                    topLeftX * 16f + 32f,
                    topLeftY * 16f - 8f
                );


            // ==========================================
            // カーソルのアイテムを全部消費
            // ==========================================

            Main.mouseItem.TurnToAir();


            // ==========================================
            // カッパーマイナーコインを必要数消費
            // ==========================================

            ConsumeCopperMinerCoins(
                player,
                coinType,
                exchangeAmount);


            // ==========================================
            // 交換結果を交換機の上からドロップ
            // ==========================================

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Item.NewItem(
                    new EntitySource_TileInteraction(
                        player,
                        topLeftX,
                        topLeftY
                    ),
                    dropPosition,
                    outputType,
                    exchangeAmount
                );
            }


            // ==========================================
            // 効果音
            // ==========================================

            SoundEngine.PlaySound(
                SoundID.Item37,
                dropPosition);


            return true;
        }


        // =========================================================
        // カッパーマイナーコイン検索
        // =========================================================

        private int FindCopperMinerCoin(
            Player player,
            int coinType)
        {
            for (int i = 0;
                 i < player.inventory.Length;
                 i++)
            {
                Item item =
                    player.inventory[i];


                if (item == null ||
                    item.IsAir)
                {
                    continue;
                }


                if (item.type != coinType)
                    continue;


                if (item.stack <= 0)
                    continue;


                return i;
            }


            return -1;
        }


        // =========================================================
        // 4×4交換機の左上座標を取得
        // =========================================================

        private void GetMachineTopLeft(
            int tileX,
            int tileY,
            out int topLeftX,
            out int topLeftY)
        {
            Tile tile =
                Main.tile[tileX, tileY];


            // TileFrameX / 18 から
            // 4×4オブジェクト内の位置を計算
            int frameX =
                tile.TileFrameX / 18;

            int frameY =
                tile.TileFrameY / 18;


            int localX =
                frameX % 4;

            int localY =
                frameY % 4;


            topLeftX =
                tileX - localX;

            topLeftY =
                tileY - localY;
        }
        private int CountCopperMinerCoins(
    Player player,
    int coinType)
        {
            int total = 0;

            for (int i = 0;
                 i < player.inventory.Length;
                 i++)
            {
                Item item = player.inventory[i];

                if (item == null ||
                    item.IsAir)
                {
                    continue;
                }

                if (item.type != coinType)
                    continue;

                total += item.stack;
            }

            return total;
        }
        private void ConsumeCopperMinerCoins(
    Player player,
    int coinType,
    int amount)
        {
            int remaining = amount;

            for (int i = 0;
                 i < player.inventory.Length;
                 i++)
            {
                if (remaining <= 0)
                    break;

                Item item = player.inventory[i];

                if (item == null ||
                    item.IsAir)
                {
                    continue;
                }

                if (item.type != coinType)
                    continue;

                int consume =
                    System.Math.Min(
                        item.stack,
                        remaining);

                item.stack -= consume;

                remaining -= consume;

                if (item.stack <= 0)
                {
                    item.TurnToAir();
                }
            }
        }
    }
}