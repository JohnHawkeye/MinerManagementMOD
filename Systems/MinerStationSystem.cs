using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace MinerManagementMOD.Systems
{
    public class MinerStationSystem : ModSystem
    {
        // ==========================================
        // Miner Base
        // ==========================================

        // Baseはワールドに1つだけ
        public static Point16 MinerBasePosition =
            new Point16(-1, -1);


        // ==========================================
        // Miner Outpost
        // ==========================================

        // Outpostは複数設置可能
        public static List<Point16> OutpostPositions =
            new List<Point16>();

        public static bool SelectingOutpost = false;

        // ==========================================
        // ワールド読み込み開始時
        // ==========================================

        public override void OnWorldLoad()
        {
            MinerBasePosition =
                new Point16(-1, -1);

            OutpostPositions.Clear();

            SelectingOutpost = false;
        }


        // ==========================================
        // ワールド終了時
        // ==========================================

        public override void OnWorldUnload()
        {
            MinerBasePosition =
                new Point16(-1, -1);

            OutpostPositions.Clear();

            SelectingOutpost = false;
        }


        // ==========================================
        // ワールド保存
        // ==========================================

        public override void SaveWorldData(TagCompound tag)
        {
            // ------------------------------------------
            // Miner Base
            // ------------------------------------------

            if (MinerBasePosition.X >= 0 &&
                MinerBasePosition.Y >= 0)
            {
                tag["MinerBaseX"] =
                    (int)MinerBasePosition.X;

                tag["MinerBaseY"] =
                    (int)MinerBasePosition.Y;
            }


            // ------------------------------------------
            // Miner Outpost
            // ------------------------------------------

            if (OutpostPositions.Count > 0)
            {
                int[] outpostX =
                    new int[OutpostPositions.Count];

                int[] outpostY =
                    new int[OutpostPositions.Count];


                for (int i = 0; i < OutpostPositions.Count; i++)
                {
                    outpostX[i] =
                        OutpostPositions[i].X;

                    outpostY[i] =
                        OutpostPositions[i].Y;
                }


                // int[]として保存する
                tag["MinerOutpostX"] = outpostX;
                tag["MinerOutpostY"] = outpostY;
            }
        }


        // ==========================================
        // ワールド読み込み
        // ==========================================

        public override void LoadWorldData(TagCompound tag)
        {
            // 念のため初期化
            MinerBasePosition =
                new Point16(-1, -1);

            OutpostPositions.Clear();


            // ==========================================
            // Miner Base
            // ==========================================

            if (tag.ContainsKey("MinerBaseX") &&
                tag.ContainsKey("MinerBaseY"))
            {
                int x =
                    tag.GetInt("MinerBaseX");

                int y =
                    tag.GetInt("MinerBaseY");


                MinerBasePosition =
                    new Point16(x, y);
            }


            // ==========================================
            // Miner Outpost
            // ==========================================

            if (tag.ContainsKey("MinerOutpostX") &&
                tag.ContainsKey("MinerOutpostY"))
            {
                LoadOutpostPositions(tag);
            }
        }


        // ==========================================
        // Outpost読み込み
        // ==========================================

        private void LoadOutpostPositions(TagCompound tag)
        {
            int[] outpostX = GetIntArraySafely(
                tag,
                "MinerOutpostX"
            );

            int[] outpostY = GetIntArraySafely(
                tag,
                "MinerOutpostY"
            );


            if (outpostX == null ||
                outpostY == null)
            {
                return;
            }


            int count = Math.Min(
                outpostX.Length,
                outpostY.Length
            );


            for (int i = 0; i < count; i++)
            {
                OutpostPositions.Add(
                    new Point16(
                        outpostX[i],
                        outpostY[i]
                    )
                );
            }
        }


        // ==========================================
        // int[]を安全に読み込む
        // ==========================================

        private int[] GetIntArraySafely(
            TagCompound tag,
            string key)
        {
            if (!tag.ContainsKey(key))
            {
                return null;
            }


            object value = tag[key];


            // ------------------------------------------
            // 現在の保存形式
            // ------------------------------------------

            if (value is int[] intArray)
            {
                return intArray;
            }


            // ------------------------------------------
            // 以前のList<int>形式
            // ------------------------------------------

            if (value is IList<int> intList)
            {
                int[] result =
                    new int[intList.Count];


                for (int i = 0; i < intList.Count; i++)
                {
                    result[i] = intList[i];
                }


                return result;
            }


            // ------------------------------------------
            // それ以外は無視
            // ------------------------------------------

            return null;
        }


        // ==========================================
        // Base登録
        // ==========================================

        public static void RegisterBase(
            int x,
            int y)
        {
            MinerBasePosition =
                new Point16(x, y);
        }


        // ==========================================
        // Base登録解除
        // ==========================================

        public static void UnregisterBase(
            int x,
            int y)
        {
            if (MinerBasePosition.X == x &&
                MinerBasePosition.Y == y)
            {
                MinerBasePosition =
                    new Point16(-1, -1);
            }
        }


        // ==========================================
        // Outpost登録
        // ==========================================

        public static void RegisterOutpost(
            int x,
            int y)
        {
            Point16 position =
                new Point16(x, y);


            if (!OutpostPositions.Contains(position))
            {
                OutpostPositions.Add(position);
            }
        }


        // ==========================================
        // Outpost登録解除
        // ==========================================

        public static void UnregisterOutpost(
            int x,
            int y)
        {
            Point16 position =
                new Point16(x, y);


            OutpostPositions.Remove(position);
        }
    }
}