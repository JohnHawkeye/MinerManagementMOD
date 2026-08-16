using System;
using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using MinerManagementMOD.Tiles;

namespace MinerManagementMOD.TileEntities
{
    public class OreSeedTileEntity : ModTileEntity
    {
        // ============================================
        // 成長設定
        // ============================================

        // 2時間 = 7200 tick
        private const int GrowthInterval = 7200;

        // 最大4段階
        private const int MaxGrowthStage = 4;

        // ============================================
        // 保存するデータ
        // ============================================

        // 最終的に変化する鉱石
        public int TargetOre { get; private set; }

        // 0～4
        public int GrowthStage { get; private set; }

        // このOreSeed固有の乱数
        // 鉱床の形を決定する
        public int Seed { get; private set; }

        // 現在の成長タイマー
        // 0～7199
        private int growthTimer;

        // ============================================
        // TileEntityとして有効か
        // ============================================

        public override bool IsTileValidForEntity(int x, int y)
        {
            if (!WorldGen.InWorld(x, y, 1))
                return false;

            Tile tile = Main.tile[x, y];

            return tile.HasTile &&
                   tile.TileType == ModContent.TileType<OreSeedTile>();
        }

        // ============================================
        // 毎tick処理
        // ============================================

        public override void Update()
        {
            // 完成済みなら何もしない
            if (GrowthStage >= MaxGrowthStage)
                return;

            growthTimer++;

            // 2時間経過していない
            if (growthTimer < GrowthInterval)
                return;

            // 余ったtickを維持
            growthTimer -= GrowthInterval;

            Grow();
        }

        // ============================================
        // 設置時の初期化
        // ============================================

        public void Initialize()
        {
            TargetOre = GetRandomOre();

            GrowthStage = 0;

            Seed = Main.rand.Next();

            growthTimer = 0;
        }

        // ============================================
        // 鉱石抽選
        // ============================================

        private int GetRandomOre()
        {
            int[] ores =
            {
                TileID.Copper,
                TileID.Tin,

                TileID.Iron,
                TileID.Lead,

                TileID.Silver,
                TileID.Tungsten,

                TileID.Gold,
                TileID.Platinum
            };

            return ores[Main.rand.Next(ores.Length)];
        }

        // ============================================
        // 1段階成長
        // ============================================

        public void Grow()
        {
            if (GrowthStage >= MaxGrowthStage)
                return;

            GrowthStage++;

            // Stageに応じた半径
            int radius = GrowthStage;

            GrowCircle(radius);

            // Stage 4で完成
            if (GrowthStage >= MaxGrowthStage)
            {
                TransformCenterToOre();
            }
            else
            {
                Sync();
            }
        }

        // ============================================
        // 円形＋ランダムないびつな形
        // ============================================

        private void GrowCircle(int radius)
        {
            int centerX = Position.X;
            int centerY = Position.Y;

            for (int dx = -radius; dx <= radius; dx++)
            {
                for (int dy = -radius; dy <= radius; dy++)
                {
                    // 中心は最後までOreSeedとして残す
                    if (dx == 0 && dy == 0)
                        continue;

                    // 円形判定
                    if (dx * dx + dy * dy > radius * radius)
                        continue;

                    // Seedによって一部をランダムに削る
                    if (!ShouldGrowHere(dx, dy, GrowthStage))
                        continue;

                    int x = centerX + dx;
                    int y = centerY + dy;

                    if (!WorldGen.InWorld(x, y, 1))
                        continue;

                    Tile tile = Main.tile[x, y];

                    // Stoneだけを鉱石に変える
                    if (!tile.HasTile)
                        continue;

                    if (tile.TileType != TileID.Stone)
                        continue;

                    tile.TileType = (ushort)TargetOre;

                    tile.TileFrameX = 0;
                    tile.TileFrameY = 0;

                    WorldGen.SquareTileFrame(x, y);
                }
            }

            if (Main.netMode == NetmodeID.Server)
            {
                int size = radius * 2 + 1;

                NetMessage.SendTileSquare(
                    -1,
                    centerX,
                    centerY,
                    size
                );
            }
        }

        // ============================================
        // Seedを利用したランダム形状
        // ============================================

        private bool ShouldGrowHere(
            int dx,
            int dy,
            int stage)
        {
            // 4方向の隣接マスは必ず成長させる。
            // これによって中心から孤立しにくくする。
            if (Math.Abs(dx) + Math.Abs(dy) == 1)
                return true;

            // 座標とSeedから決定論的な値を作る
            unchecked
            {
                int value =
                    Seed
                    + dx * 374761393
                    + dy * 668265263
                    + stage * 1442695041;

                value ^= value >> 13;
                value *= 1274126177;
                value ^= value >> 16;

                // 0～99
                int chance = Math.Abs(value % 100);

                // 約82%の確率で残す
                return chance < 82;
            }
        }

        // ============================================
        // 中心を鉱石へ変換
        // ============================================

        private void TransformCenterToOre()
        {
            int x = Position.X;
            int y = Position.Y;

            if (!WorldGen.InWorld(x, y, 1))
            {
                RemoveAt(x, y);
                return;
            }

            Tile tile = Main.tile[x, y];

            // OreSeedが既に無い場合
            if (!tile.HasTile ||
                tile.TileType != ModContent.TileType<OreSeedTile>())
            {
                RemoveAt(x, y);
                return;
            }

            tile.TileType = (ushort)TargetOre;

            tile.TileFrameX = 0;
            tile.TileFrameY = 0;

            WorldGen.SquareTileFrame(x, y);

            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendTileSquare(
                    -1,
                    x,
                    y,
                    1
                );
            }

            // 完成したのでTileEntityを削除
            RemoveAt(x, y);
        }

        // ============================================
        // TileEntity同期
        // ============================================

        private void Sync()
        {
            if (Main.netMode != NetmodeID.Server)
                return;

            NetMessage.SendData(
                MessageID.TileEntitySharing,
                -1,
                -1,
                null,
                ID
            );
        }

        // ============================================
        // Save
        // ============================================

        public override void SaveData(TagCompound tag)
        {
            tag["TargetOre"] = TargetOre;
            tag["GrowthStage"] = GrowthStage;
            tag["Seed"] = Seed;
            tag["GrowthTimer"] = growthTimer;
        }

        // ============================================
        // Load
        // ============================================

        public override void LoadData(TagCompound tag)
        {
            TargetOre = tag.GetInt("TargetOre");
            GrowthStage = tag.GetInt("GrowthStage");
            Seed = tag.GetInt("Seed");
            growthTimer = tag.GetInt("GrowthTimer");
        }

        // ============================================
        // Multiplayer送信
        // ============================================

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(TargetOre);
            writer.Write(GrowthStage);
            writer.Write(Seed);
            writer.Write(growthTimer);
        }

        // ============================================
        // Multiplayer受信
        // ============================================

        public override void NetReceive(BinaryReader reader)
        {
            TargetOre = reader.ReadInt32();
            GrowthStage = reader.ReadInt32();
            Seed = reader.ReadInt32();
            growthTimer = reader.ReadInt32();
        }

        // ============================================
        // TileEntity存在確認
        // ============================================

        public static bool ExistsAt(int x, int y)
        {
            return TileEntity.ByPosition.TryGetValue(
                new Point16(x, y),
                out TileEntity entity
            )
            && entity is OreSeedTileEntity;
        }

        // ============================================
        // TileEntity設置
        // ============================================

        public static void PlaceAt(int x, int y)
        {
            if (!WorldGen.InWorld(x, y, 1))
                return;

            if (ExistsAt(x, y))
                return;

            OreSeedTileEntity tileEntity =
                ModContent.GetInstance<OreSeedTileEntity>();

            // Place()はModTileEntityのインスタンスメソッド
            int entityID = tileEntity.Place(x, y);

            if (entityID < 0)
                return;

            if (TileEntity.ByID.TryGetValue(
                entityID,
                out TileEntity baseEntity)
                && baseEntity is OreSeedTileEntity entity)
            {
                entity.Initialize();

                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(
                        MessageID.TileEntitySharing,
                        -1,
                        -1,
                        null,
                        entity.ID
                    );
                }
            }
        }

        // ============================================
        // TileEntity削除
        // ============================================

        public static void RemoveAt(int x, int y)
        {
            if (!WorldGen.InWorld(x, y, 1))
                return;

            OreSeedTileEntity tileEntity =
                ModContent.GetInstance<OreSeedTileEntity>();

            // Kill()もModTileEntityのインスタンスメソッド
            tileEntity.Kill(x, y);
        }
    }
}