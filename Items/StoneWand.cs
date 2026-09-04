using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace MinerManagementMOD.Items
{
    public class StoneWand : ModItem
    {
        // 連続設置の間隔
        private const int UseCooldown = 3;

        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(
                Item.type,
                new DrawAnimationVertical(
                    8,  // 1フレームの時間
                    3   // フレーム数
                )
            );
        }

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 32;

            Item.useStyle = ItemUseStyleID.HoldUp;

            Item.useTime = UseCooldown;
            Item.useAnimation = UseCooldown;

            // 押しっぱなしで連続使用
            Item.autoReuse = true;
            Item.channel = true;

            Item.noMelee = true;

            Item.maxStack = 1;

            Item.value = Item.buyPrice(platinum: 5);
            Item.rare = ItemRarityID.Blue;

            // 通常のUseSoundは使用しない
            // 実際に設置できた場所で個別に音を鳴らす
            Item.UseSound = null;
        }

        public override bool CanUseItem(Player player)
        {
            // Stoneを1個以上持っている場合のみ使用可能
            return player.CountItem(ItemID.StoneBlock) > 0;
        }

        public override bool? UseItem(Player player)
        {
            Point center = Main.MouseWorld.ToTileCoordinates();

            int placedCount = PlaceStone3x3(center.X, center.Y);

            if (placedCount <= 0)
                return false;

            // 実際に置けた数だけStoneを消費
            ConsumeStone(player, placedCount);

            // 設置演出
            PlayPlaceEffects(center.X, center.Y);

            return true;
        }

        /// <summary>
        /// 指定位置を中心に3×3へStoneを設置します。
        /// </summary>
        private int PlaceStone3x3(int centerX, int centerY)
        {
            int placedCount = 0;

            for (int offsetX = -1; offsetX <= 1; offsetX++)
            {
                for (int offsetY = -1; offsetY <= 1; offsetY++)
                {
                    int tileX = centerX + offsetX;
                    int tileY = centerY + offsetY;

                    if (!WorldGen.InWorld(tileX, tileY, 1))
                        continue;

                    Tile tile = Framing.GetTileSafely(tileX, tileY);

                    // すでにタイルが存在する場所には置かない
                    if (tile.HasTile)
                        continue;

                    // 液体がある場所にも置かない
                    if (tile.LiquidAmount > 0)
                        continue;

                    // Stoneを設置
                    WorldGen.PlaceTile(
                        tileX,
                        tileY,
                        TileID.Stone,
                        mute: true,
                        forced: true
                    );

                    // 本当にStoneが設置されたか確認
                    Tile resultTile = Framing.GetTileSafely(tileX, tileY);

                    if (resultTile.HasTile &&
                        resultTile.TileType == TileID.Stone)
                    {
                        placedCount++;

                        // マルチプレイ時の同期
                        if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.SendTileSquare(
                                -1,
                                tileX,
                                tileY,
                                1
                            );
                        }
                    }
                }
            }

            return placedCount;
        }

        /// <summary>
        /// Stoneを指定数だけ消費します。
        /// インベントリ上部から順番に消費します。
        /// </summary>
        private void ConsumeStone(Player player, int amount)
        {
            int remaining = amount;

            for (int i = 0; i < player.inventory.Length; i++)
            {
                Item item = player.inventory[i];

                if (item.type != ItemID.StoneBlock)
                    continue;

                if (item.stack <= 0)
                    continue;

                int consumeAmount = Math.Min(item.stack, remaining);

                item.stack -= consumeAmount;
                remaining -= consumeAmount;

                if (item.stack <= 0)
                    item.TurnToAir();

                if (remaining <= 0)
                    break;
            }
        }

        /// <summary>
        /// Stone設置時の煙・石の音。
        /// </summary>
        private void PlayPlaceEffects(int centerX, int centerY)
        {
            Vector2 centerPosition = new Vector2(
                centerX * 16f + 8f,
                centerY * 16f + 8f
            );

            // 石を置いた音
            SoundEngine.PlaySound(
                SoundID.Tink,
                centerPosition
            );

            // 3×3の設置範囲全体から煙を出す
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    int tileX = centerX + x;
                    int tileY = centerY + y;

                    Tile tile = Framing.GetTileSafely(tileX, tileY);

                    if (!tile.HasTile || tile.TileType != TileID.Stone)
                        continue;

                    Vector2 position = new Vector2(
                        tileX * 16f + 8f,
                        tileY * 16f + 8f
                    );

                    for (int i = 0; i < 2; i++)
                    {
                        Dust dust = Dust.NewDustPerfect(
                            position + Main.rand.NextVector2Circular(5f, 5f),
                            DustID.Stone,
                            Main.rand.NextVector2Circular(1.2f, 1.2f),
                            100,
                            default,
                            Main.rand.NextFloat(0.7f, 1.1f)
                        );

                        dust.noGravity = false;
                    }
                }
            }
        }

    }
}