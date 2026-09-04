using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ID;
using MinerManagementMOD.NPCs;
using MinerManagementMOD.Buffs;
using Microsoft.Xna.Framework;

namespace MinerManagementMOD.Systems
{
    public class RainbowBuffPlayer : ModPlayer
    {
        // ============================================================
        // 保存データ
        // ============================================================

        // 累計レインボージュエル奉納数
        public int RainbowJewelCount;

        // 現在のレベル
        public int RainbowLevel = 1;


        // ============================================================
        // 一時的な状態
        // ============================================================

        // 今この瞬間、ジュエルドラゴンが近くにいるか
        public bool RainbowBuffActive;


        // ============================================================
        // レベルごとの能力値
        // ============================================================

        // 与ダメージ
        private static readonly float[] DamageBonus =
        {
            0f,
            0.10f,  // Lv1
            0.20f,  // Lv2
            0.30f,  // Lv3
            0.40f,  // Lv4
            0.50f,  // Lv5
            0.75f,  // Lv6
            1.00f   // Lv7
        };

        // 被ダメージ軽減
        private static readonly float[] DamageReduction =
        {
            0f,
            0.05f,
            0.10f,
            0.15f,
            0.20f,
            0.30f,
            0.40f,
            0.50f
        };

        // 採掘速度
        private static readonly float[] MiningSpeedBonus =
        {
            0f,
            0.03f,
            0.06f,
            0.09f,
            0.12f,
            0.15f,
            0.20f,
            0.25f
        };

        // 移動速度
        private static readonly float[] MoveSpeedBonus =
        {
            0f,
            0.03f,
            0.06f,
            0.09f,
            0.12f,
            0.15f,
            0.20f,
            0.25f
        };

        // 攻撃速度
        private static readonly float[] AttackSpeedBonus =
        {
            0f,
            0.03f,
            0.06f,
            0.09f,
            0.12f,
            0.15f,
            0.20f,
            0.25f
        };

        // マナ消費軽減
        private static readonly float[] ManaCostReduction =
        {
            0f,
            0.05f,
            0.10f,
            0.15f,
            0.20f,
            0.30f,
            0.40f,
            0.50f
        };


        // ============================================================
        // 初期化
        // ============================================================

        public override void Initialize()
        {
            RainbowJewelCount = 0;
            RainbowLevel = 1;
            RainbowBuffActive = false;
        }

        // ============================================================
        // プレイヤー更新
        // ============================================================

        public override void PreUpdate()
        {
            RainbowBuffActive = IsJewelDragonSummoned();

            // if (Main.GameUpdateCount % 60 == 0)
            // {
            //     Main.NewText(
            //         $"Dragon={RainbowBuffActive}",
            //         255,
            //         100,
            //         255
            //     );
            // }

            if (RainbowBuffActive)
            {
                RainbowLevel = CalculateLevel(RainbowJewelCount);

                Player.AddBuff(
                    ModContent.BuffType<RainbowBuff>(),
                    600,
                    true
                );
            }
        }


        // ============================================================
        // 各種能力
        // ============================================================

        public override void PostUpdateMiscEffects()
        {
            if (!RainbowBuffActive)
                return;

            int level = Math.Clamp(RainbowLevel, 1, 7);

            // --------------------------------------------------------
            // 移動速度
            // --------------------------------------------------------

            Player.moveSpeed += MoveSpeedBonus[level];

            // --------------------------------------------------------
            // 採掘速度
            //
            // pickSpeed は値が小さいほど速い
            // --------------------------------------------------------

            Player.pickSpeed -= MiningSpeedBonus[level];

            // --------------------------------------------------------
            // 攻撃速度
            // --------------------------------------------------------

            Player.GetAttackSpeed(DamageClass.Generic)
                += AttackSpeedBonus[level];
        }


        // ============================================================
        // 与えるダメージ
        // ============================================================

        public override void ModifyHitNPC(
            NPC target,
            ref NPC.HitModifiers modifiers)
        {
           
            if (!RainbowBuffActive)
                return;

            int level = Math.Clamp(RainbowLevel, 1, 7);

            modifiers.SourceDamage *=
                1f + DamageBonus[level];


        }


        public override void ModifyHitNPCWithItem(
            Item item,
            NPC target,
            ref NPC.HitModifiers modifiers)
        {
            if (!RainbowBuffActive)
                return;

            int level = Math.Clamp(RainbowLevel, 1, 7);

            modifiers.SourceDamage *=
                1f + DamageBonus[level];
        }


        public override void ModifyHitNPCWithProj(
            Projectile proj,
            NPC target,
            ref NPC.HitModifiers modifiers)
        {
            if (!RainbowBuffActive)
                return;

            int level = Math.Clamp(RainbowLevel, 1, 7);

            modifiers.SourceDamage *=
                1f + DamageBonus[level];
        }


        // ============================================================
        // 受けるダメージ
        // ============================================================

        public override void ModifyHurt(
            ref Player.HurtModifiers modifiers)
        {
            if (!RainbowBuffActive)
                return;

            int level = Math.Clamp(RainbowLevel, 1, 7);

            // 例：
            // Lv1 = 5%軽減 → 0.95倍
            // Lv7 = 50%軽減 → 0.50倍

            modifiers.FinalDamage *=
                1f - DamageReduction[level];
        }


        // ============================================================
        // マナ消費
        // ============================================================

        public override void ModifyManaCost(
            Item item,
            ref float reduce,
            ref float mult)
        {
            if (!RainbowBuffActive)
                return;

            int level = Math.Clamp(RainbowLevel, 1, 7);

            mult *=
                1f - ManaCostReduction[level];
        }


        // ============================================================
        // ジュエルドラゴンが近くにいるか
        // ============================================================

        private bool IsJewelDragonSummoned()
        {
            foreach (NPC npc in Main.npc)
            {
                if (!npc.active)
                    continue;

                if (npc.ModNPC is JewelDragonNPC)
                    return true;

            }

            return false;
        }


        // ============================================================
        // 奉納によるレベル計算
        // ============================================================

        public static int CalculateLevel(int count)
        {
            if (count < 20)
                return 1;

            if (count < 60)
                return 2;

            if (count < 140)
                return 3;

            if (count < 300)
                return 4;

            if (count < 620)
                return 5;

            if (count < 1260)
                return 6;

            return 7;
        }


        // ============================================================
        // 次のレベルまで必要な奉納数
        // ============================================================

        public static int GetRequiredForNextLevel(int level)
        {
            switch (level)
            {
                case 1:
                    return 20;

                case 2:
                    return 40;

                case 3:
                    return 80;

                case 4:
                    return 160;

                case 5:
                    return 320;

                case 6:
                    return 640;

                default:
                    return 0;
            }
        }


        // ============================================================
        // 現在レベル内での奉納数
        // ============================================================

        public int GetCurrentLevelProgress()
        {
            switch (RainbowLevel)
            {
                case 1:
                    return RainbowJewelCount;

                case 2:
                    return RainbowJewelCount - 20;

                case 3:
                    return RainbowJewelCount - 60;

                case 4:
                    return RainbowJewelCount - 140;

                case 5:
                    return RainbowJewelCount - 300;

                case 6:
                    return RainbowJewelCount - 620;

                case 7:
                    return 0;

                default:
                    return 0;
            }
        }


        // ============================================================
        // レインボージュエルを1個奉納
        // ============================================================

        public bool OfferRainbowJewel()
        {
            int itemType =
                ModContent.ItemType<Items.RainbowJewel>();

            if (Player.CountItem(itemType) <= 0)
                return false;

            // 最大レベルなら奉納しない
            if (RainbowLevel >= 7)
                return false;

            Player.ConsumeItem(itemType);

            RainbowJewelCount++;

            int oldLevel = RainbowLevel;

            RainbowLevel =
                CalculateLevel(RainbowJewelCount);

            if (RainbowLevel > oldLevel)
            {
                Main.NewText(
                    $"レインボーバフがLv.{RainbowLevel}になりました！",
                    150, 255, 255
                );
            }
            else
            {
                Main.NewText(
                    $"レインボージュエルを1個捧げました。"
                    + $"（累計 {RainbowJewelCount}個）",
                    200, 255, 200
                );
            }

            return true;
        }


        // ============================================================
        // セーブ
        // ============================================================

        public override void SaveData(TagCompound tag)
        {
            if (RainbowJewelCount > 0)
            {
                tag["RainbowJewelCount"] =
                    RainbowJewelCount;
            }
        }


        // ============================================================
        // ロード
        // ============================================================

        public override void LoadData(TagCompound tag)
        {
            RainbowJewelCount =
                tag.GetInt("RainbowJewelCount");

            RainbowLevel =
                CalculateLevel(RainbowJewelCount);
        }
    }
}