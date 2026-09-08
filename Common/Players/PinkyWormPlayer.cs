using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace MinerManagementMOD.Common.Players
{
    public class PinkyWormPlayer : ModPlayer
    {
        // =========================================================
        // 基本設定
        // =========================================================

        public const int HitDamage = 20;

        public const float BaseKnockBack = 8f;
        public const float MaxKnockBack = 10f;

        public const int HitCooldown = 30;

        // 10個の鉱石を拾うごとに身体が1節成長
        public const int OrePerBody = 10;

        // 最大身体節数
        public const int MaxBodySegments = 30;

        // 最大速度
        public const float MaxSpeed = 8f;

        // 加速度
        public const float Acceleration = 0.20f;

        // 左右旋回速度
        public const float TurnSpeed = 0.055f;

        // 下入力による減速
        public const float BrakeAmount = 0.18f;

        // 身体節間の距離
        public const float SegmentDistance = 36f;

        // 地上へ出すぎないための余白
        public const int UndergroundMargin = 10;

        private Vector2 wormPosition;

        // =========================================================
        // ワーム状態
        // =========================================================

        public bool PinkyWormActive;

        public bool IsMoving =>
            PinkyWormActive &&
            WormSpeed > 0.15f;

        // 現在速度
        public float WormSpeed;

        // ワームの向き
        public float WormRotation;

        // 現在までに拾った鉱石数
        public int CollectedOre;

        // 現在の身体節数
        public int BodySegments;


        // =========================================================
        // 身体位置履歴
        // =========================================================

        private readonly List<Vector2> positionHistory =
            new List<Vector2>();

        public IReadOnlyList<Vector2> PositionHistory =>
            positionHistory;


        // =========================================================
        // NPCヒットクールダウン
        // =========================================================

        private readonly Dictionary<int, int> hitCooldowns =
            new Dictionary<int, int>();

        //sound option
        private int wormSoundTimer;
        public const int MinWormSoundInterval = 8;
        public const int MaxWormSoundInterval = 35;

        // =========================================================
        // 初期化
        // =========================================================

        public override void Initialize()
        {
            PinkyWormActive = false;

            WormSpeed = 0f;
            WormRotation = 0f;

            CollectedOre = 0;
            BodySegments = 0;

            wormSoundTimer = 0;

            positionHistory.Clear();
            hitCooldowns.Clear();
        }


        // =========================================================
        // ワーム開始
        // =========================================================

        public void StartWorm()
        {
            PinkyWormActive = true;

            WormSpeed = 0f;
            wormSoundTimer = 0;

            // 成長状態は毎回リセット
            BodySegments = 0;
            CollectedOre = 0;

            positionHistory.Clear();

            // Playerの向きをワームの初期方向にする
            WormRotation =
                Player.direction == -1
                    ? MathF.PI
                    : 0f;

            // 最初から十分な履歴を用意
            Vector2 center = Player.Center;

            for (int i = 0; i < 300; i++)
            {
                positionHistory.Add(center);
            }

            hitCooldowns.Clear();

            // ワームの当たり判定サイズ
            Player.width = 32;
            Player.height = 32;

            // 落下ダメージ無効
            Player.noFallDmg = true;

            // 通常Playerの速度を停止
            Player.velocity = Vector2.Zero;

            // 重力停止
            Player.gravity = 0f;
            wormPosition = Player.position;
        }


        // =========================================================
        // ワーム終了
        // =========================================================

        public void StopWorm()
        {
            PinkyWormActive = false;

            WormSpeed = 0f;
            wormSoundTimer = 0;

            // 成長は一時的なもの
            BodySegments = 0;
            CollectedOre = 0;

            positionHistory.Clear();
            hitCooldowns.Clear();

            // 通常Playerへ戻す
            Player.velocity = Vector2.Zero;

            // 次の通常Player更新で通常状態へ戻す
            Player.gravity = 0f;
        }


        // =========================================================
        // PreUpdate
        // =========================================================

        public override void PreUpdate()
        {

            if (!PinkyWormActive)
                return;

            if (Player.dead)
            {
                if (Player.mount.Active)
                {
                    Player.mount.Dismount(Player);
                }

                return;
            }

            Player.position = wormPosition;

            Player.gravity = 0f;
            Player.velocity = Vector2.Zero;
            Player.noFallDmg = true;

            UpdateHitCooldowns();
        }


        // =========================================================
        // 通常Player移動を停止
        // =========================================================
        //
        // PreUpdateMovementは、Playerがvelocityを使って
        // 通常移動する直前に呼ばれる。
        //
        // ここでは通常移動を完全にゼロにする。
        //
        // ワーム自身の移動はPostUpdateで行う。
        // =========================================================

        public override void PreUpdateMovement()
        {
            if (!PinkyWormActive)
                return;

            // 通常速度を完全停止
            Player.velocity = Vector2.Zero;

            // 重力を無効化
            Player.gravity = 0f;

            // 前フレーム速度も停止
            Player.oldVelocity = Vector2.Zero;
        }


        // =========================================================
        // Player.Updateの最後
        // =========================================================
        //
        // vanillaのPlayer.Updateがすべて終了したあとに
        // ワーム独自の移動を行う。
        //
        // PostUpdateはPlayer.Updateの最後に呼ばれる。
        // =========================================================

        public override void PostUpdate()
        {
            if (!PinkyWormActive)
                return;

            if (Player.dead)
                return;

            // -----------------------------------------------------
            // vanilla側から変更された速度・重力を再び停止
            // -----------------------------------------------------

            Player.velocity = Vector2.Zero;
            Player.gravity = 0f;

            // -----------------------------------------------------
            // 地上へ出すぎないようにする
            // -----------------------------------------------------

            float surfaceY =
                (float)(Main.worldSurface * 16.0);

            if (Player.Center.Y <
                surfaceY + UndergroundMargin * 16f)
            {
                WormSpeed = 0f;

                Vector2 correctedCenter =
                    Player.Center;

                correctedCenter.Y =
                    surfaceY +
                    UndergroundMargin * 16f;

                Player.Center =
                    correctedCenter;

                return;
            }

            // -----------------------------------------------------
            // 左旋回
            // -----------------------------------------------------

            if (Player.controlLeft)
            {
                WormRotation -= TurnSpeed;
            }

            // -----------------------------------------------------
            // 右旋回
            // -----------------------------------------------------

            if (Player.controlRight)
            {
                WormRotation += TurnSpeed;
            }

            // -----------------------------------------------------
            // 前進
            // -----------------------------------------------------

            if (Player.controlUp)
            {
                WormSpeed += Acceleration;

                WormSpeed =
                    MathHelper.Clamp(
                        WormSpeed,
                        0f,
                        MaxSpeed
                    );
            }

            // -----------------------------------------------------
            // 下入力
            // -----------------------------------------------------

            if (Player.controlDown)
            {
                WormSpeed -= BrakeAmount;

                WormSpeed =
                    MathHelper.Clamp(
                        WormSpeed,
                        0f,
                        MaxSpeed
                    );
            }

            // -----------------------------------------------------
            // 入力なしで徐々に減速
            // -----------------------------------------------------

            if (!Player.controlUp &&
                !Player.controlDown)
            {
                WormSpeed *= 0.985f;

                if (WormSpeed < 0.05f)
                {
                    WormSpeed = 0f;
                }
            }

            // -----------------------------------------------------
            // 移動方向
            // -----------------------------------------------------

            Vector2 direction =
                new Vector2(
                    MathF.Cos(WormRotation),
                    MathF.Sin(WormRotation)
                );

            Vector2 movement =
                direction * WormSpeed;

            wormPosition += movement;

            Player.position = wormPosition;

            Player.velocity = Vector2.Zero;

            // -----------------------------------------------------
            // Playerの向き
            // -----------------------------------------------------

            Player.direction =
                MathF.Cos(WormRotation) >= 0f
                    ? 1
                    : -1;

            UpdatePositionHistory(Player.Center);
            TryMineOre();
            TryAttackNPCs();
            UpdateWormSound();
        }


        // =========================================================
        // Playerの通常描画を完全に非表示
        // =========================================================
        //
        // PlayerDrawLayerLoader.Layersに登録されている
        // 全描画レイヤーを非表示にする。
        //
        // ただしPinkyWormDrawLayerだけは除外する。
        //
        // これにより、
        //
        // Player本体
        // 髪
        // 頭
        // 防具
        // 腕
        // 脚
        // 手持ちアイテム
        // Wings
        // Mountの通常描画
        // その他のPlayer描画
        //
        // を描画しない。
        // =========================================================

        public override void HideDrawLayers(
            Terraria.DataStructures.PlayerDrawSet drawInfo)
        {
            if (!PinkyWormActive)
                return;

            PlayerDrawLayer wormLayer =
                ModContent.GetInstance<
                    Systems.PinkyWormDrawLayer>();

            foreach (PlayerDrawLayer layer
                     in PlayerDrawLayerLoader.Layers)
            {
                // Pinky Worm自身の描画だけは残す
                if (layer == wormLayer)
                    continue;

                layer.Hide();
            }
        }


        // =========================================================
        // 身体位置履歴更新
        // =========================================================

        private void UpdatePositionHistory(
            Vector2 position)
        {
            positionHistory.Insert(
                0,
                position
            );

            int required =
                (BodySegments + 3) * 60;

            while (positionHistory.Count > required)
            {
                positionHistory.RemoveAt(
                    positionHistory.Count - 1
                );
            }
        }


        // =========================================================
        // 頭部Hitbox
        // =========================================================

        public Rectangle GetHeadHitbox()
        {
            return new Rectangle(
                (int)Player.Center.X - 16,
                (int)Player.Center.Y - 16,
                32,
                32
            );
        }


        // =========================================================
        // NPC攻撃
        // =========================================================

        private void TryAttackNPCs()
        {
            Rectangle head =
                GetHeadHitbox();

            for (int i = 0;
                 i < Main.maxNPCs;
                 i++)
            {
                NPC npc =
                    Main.npc[i];

                if (!npc.active)
                    continue;

                if (npc.friendly)
                    continue;

                if (npc.dontTakeDamage)
                    continue;

                if (npc.lifeMax <= 0)
                    continue;

                if (!head.Intersects(npc.Hitbox))
                    continue;

                // クールダウン
                if (hitCooldowns.TryGetValue(
                        i,
                        out int cooldown) &&
                    cooldown > 0)
                {
                    continue;
                }

                // -------------------------------------------------
                // 速度によるノックバック
                // -------------------------------------------------

                float speedRatio =
                    MathHelper.Clamp(
                        WormSpeed / MaxSpeed,
                        0f,
                        1f
                    );

                float knockback =
                    MathHelper.Lerp(
                        BaseKnockBack,
                        MaxKnockBack,
                        speedRatio
                    );

                // -------------------------------------------------
                // 攻撃方向
                // -------------------------------------------------

                Vector2 direction =
                    new Vector2(
                        MathF.Cos(WormRotation),
                        MathF.Sin(WormRotation)
                    );

                int hitDirection =
                    direction.X >= 0f
                        ? 1
                        : -1;

                // -------------------------------------------------
                // ダメージ
                // -------------------------------------------------

                NPC.HitInfo hit =
                    new NPC.HitInfo
                    {
                        Damage = HitDamage,
                        Knockback = knockback,
                        HitDirection = hitDirection,
                        Crit = false
                    };

                npc.StrikeNPC(
                    hit,
                    false,
                    false
                );

                // -------------------------------------------------
                // NPCを押し飛ばす
                // -------------------------------------------------

                npc.velocity +=
                    direction *
                    knockback;

                // -------------------------------------------------
                // クールダウン
                // -------------------------------------------------

                hitCooldowns[i] =
                    HitCooldown;

                // -------------------------------------------------
                // ヒットエフェクト
                // -------------------------------------------------

                for (int d = 0; d < 8; d++)
                {
                    Dust dust =
                        Dust.NewDustDirect(
                            npc.position,
                            npc.width,
                            npc.height,
                            DustID.PinkTorch
                        );

                    dust.noGravity = true;

                    dust.velocity =
                        direction *
                        Main.rand.NextFloat(
                            1f,
                            3f
                        );
                }

                Terraria.Audio.SoundEngine.PlaySound(
                    SoundID.NPCHit1,
                    npc.Center
                );
            }
        }


        // =========================================================
        // NPCクールダウン更新
        // =========================================================

        private void UpdateHitCooldowns()
        {
            if (hitCooldowns.Count == 0)
                return;

            List<int> remove =
                new List<int>();

            foreach (var pair in hitCooldowns)
            {
                int value =
                    pair.Value - 1;

                if (value <= 0)
                {
                    remove.Add(
                        pair.Key
                    );
                }
                else
                {
                    hitCooldowns[pair.Key] =
                        value;
                }
            }

            foreach (int id in remove)
            {
                hitCooldowns.Remove(id);
            }
        }


        // =========================================================
        // 鉱石採掘
        // =========================================================

        private void TryMineOre()
        {
            Rectangle head =
                GetHeadHitbox();

            int left =
                head.Left / 16 - 1;

            int right =
                head.Right / 16 + 1;

            int top =
                head.Top / 16 - 1;

            int bottom =
                head.Bottom / 16 + 1;

            for (int x = left;
                 x <= right;
                 x++)
            {
                for (int y = top;
                     y <= bottom;
                     y++)
                {
                    if (!WorldGen.InWorld(
                            x,
                            y,
                            1))
                    {
                        continue;
                    }

                    Tile tile =
                        Main.tile[x, y];

                    if (tile == null ||
                        !tile.HasTile)
                    {
                        continue;
                    }

                    // 鉱石だけを対象
                    if (!TileID.Sets.Ore[
                            tile.TileType])
                    {
                        continue;
                    }

                    Rectangle tileRect =
                        new Rectangle(
                            x * 16,
                            y * 16,
                            16,
                            16
                        );

                    if (!head.Intersects(
                            tileRect))
                    {
                        continue;
                    }

                    // 鉱石を破壊
                    WorldGen.KillTile(
                        x,
                        y
                    );
                }
            }
        }


        // =========================================================
        // 鉱石取得
        // =========================================================

        public void AddCollectedOre(
            int amount)
        {
            if (!PinkyWormActive)
                return;

            if (amount <= 0)
                return;

            CollectedOre += amount;

            while (CollectedOre >= OrePerBody)
            {
                CollectedOre -= OrePerBody;

                if (BodySegments <
                    MaxBodySegments)
                {
                    BodySegments++;

                    SpawnGrowthEffect();
                }
            }
        }


        // =========================================================
        // 成長エフェクト
        // =========================================================

        private void SpawnGrowthEffect()
        {
            for (int i = 0;
                 i < 20;
                 i++)
            {
                Dust dust =
                    Dust.NewDustDirect(
                        Player.position,
                        Player.width,
                        Player.height,
                        DustID.PinkTorch
                    );

                dust.noGravity = true;

                dust.velocity =
                    Main.rand.NextVector2Circular(
                        3f,
                        3f
                    );
            }

            Terraria.Audio.SoundEngine.PlaySound(
                SoundID.ResearchComplete,
                Player.Center
            );
        }


        // =========================================================
        // アイテム使用制限
        // =========================================================

        public override bool CanUseItem(
            Item item)
        {
            if (PinkyWormActive)
            {
                // 武器などの通常攻撃アイテムを使用不可
                if (item.damage > 0)
                    return false;
            }

            return true;
        }

        private void UpdateWormSound()
        {
            if (!PinkyWormActive)
                return;

            // 停止中は音を鳴らさない
            if (WormSpeed <= 0.15f)
            {
                wormSoundTimer = 0;
                return;
            }

            // 次の音まで待つ
            if (wormSoundTimer > 0)
            {
                wormSoundTimer--;
                return;
            }

            // 0～1に速度を変換
            float speedRatio =
                MathHelper.Clamp(
                    WormSpeed / MaxSpeed,
                    0f,
                    1f
                );

            // -------------------------------------------------
            // 速度が速いほど再生間隔を短くする
            // -------------------------------------------------

            int interval =
                (int)MathHelper.Lerp(
                    MaxWormSoundInterval,
                    MinWormSoundInterval,
                    speedRatio
                );

            // -------------------------------------------------
            // 速度が速いほど音程を高くする
            // -------------------------------------------------

            float pitch =
                MathHelper.Lerp(
                    -0.20f,
                    0.20f,
                    speedRatio
                );

            SoundStyle wormSound =
                SoundID.WormDig with
                {
                    Pitch = pitch,
                    Volume = 0.55f
                };

            SoundEngine.PlaySound(
                wormSound,
                Player.Center
            );

            wormSoundTimer = interval;
        }
    }
}