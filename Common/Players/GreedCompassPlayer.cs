using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace MinerManagementMOD.Common.Players
{
    public class GreedCompassPlayer : ModPlayer
    {
        // コンパス検索で見つかった宝箱
        // 近い順に並んでいる
        private readonly List<ChestTarget> chestTargets = new();

        // 最大表示数
        public const int MaxDisplayedChests = 5;

        public override void Initialize()
        {
            chestTargets.Clear();
        }

        public override void OnEnterWorld()
        {
            chestTargets.Clear();
        }

        public override void UpdateDead()
        {
            chestTargets.Clear();
        }

        /// <summary>
        /// 指定距離以内の宝箱を検索する。
        /// Main.chestを調べるため、ワールド全体のタイル走査は行わない。
        /// </summary>
        public bool ScanForChests(float searchRange)
        {
            chestTargets.Clear();

            Vector2 playerCenter = Player.Center;
            float searchRangeSquared = searchRange * searchRange;

            for (int i = 0; i < Main.maxChests; i++)
            {
                Chest chest = Main.chest[i];

                if (chest == null)
                    continue;

                Vector2 chestPosition = new Vector2(
                    chest.x * 16f + 16f,
                    chest.y * 16f + 16f
                );

                float distanceSquared =
                    Vector2.DistanceSquared(
                        playerCenter,
                        chestPosition
                    );

                if (distanceSquared > searchRangeSquared)
                    continue;

                chestTargets.Add(
                    new ChestTarget(
                        i,
                        new Point(chest.x, chest.y),
                        distanceSquared
                    )
                );
            }

            // 近い順に並べる
            chestTargets.Sort(
                (a, b) =>
                    a.DistanceSquared.CompareTo(
                        b.DistanceSquared
                    )
            );

            return chestTargets.Count > 0;
        }

        /// <summary>
        /// 現在有効な宝箱を取得する。
        /// 最大5個。
        /// </summary>
        public List<ChestTarget> GetActiveTargets()
        {
            RemoveInvalidTargets();

            int count = System.Math.Min(
                MaxDisplayedChests,
                chestTargets.Count
            );

            return chestTargets.GetRange(0, count);
        }

        /// <summary>
        /// 破壊された宝箱や、プレイヤーが開いた宝箱を削除する。
        /// </summary>
        private void RemoveInvalidTargets()
        {
            for (int i = chestTargets.Count - 1; i >= 0; i--)
            {
                ChestTarget target = chestTargets[i];

                // 宝箱そのものがなくなった
                if (target.ChestIndex < 0 ||
                    target.ChestIndex >= Main.maxChests)
                {
                    chestTargets.RemoveAt(i);
                    continue;
                }

                Chest chest = Main.chest[target.ChestIndex];

                if (chest == null)
                {
                    chestTargets.RemoveAt(i);
                    continue;
                }

                // 別の場所に新しい宝箱が入っていた場合
                if (chest.x != target.Position.X ||
                    chest.y != target.Position.Y)
                {
                    chestTargets.RemoveAt(i);
                    continue;
                }

                // 自分が現在開いている宝箱なら、
                // 「回収済み」としてコンパス対象から外す。
                if (Player.chest == target.ChestIndex)
                {
                    chestTargets.RemoveAt(i);
                }
            }
        }

        public void ClearTargets()
        {
            chestTargets.Clear();
        }

        public class ChestTarget
        {
            public int ChestIndex;
            public Point Position;
            public float DistanceSquared;

            public ChestTarget(
                int chestIndex,
                Point position,
                float distanceSquared)
            {
                ChestIndex = chestIndex;
                Position = position;
                DistanceSquared = distanceSquared;
            }

            public Vector2 WorldPosition
            {
                get
                {
                    return new Vector2(
                        Position.X * 16f + 16f,
                        Position.Y * 16f + 16f
                    );
                }
            }
        }
    }
}