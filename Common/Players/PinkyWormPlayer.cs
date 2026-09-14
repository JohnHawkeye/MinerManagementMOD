using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace MinerManagementMOD.Common.Players
{
    public class PinkyWormPlayer : ModPlayer
    {
        // =========================================================
        // Pinky Worm
        // =========================================================

        // 基本攻撃力
        public const int HitDamage = 20;

        // 身体1節につき増加する攻撃力
        public const int DamagePerBodySegment = 10;

        public const float BaseKnockBack = 8f;
        public const float MaxKnockBack = 10f;
        public const int HitCooldown = 30;

        // 10個の鉱石を取得するごとに身体が1節伸びる
        public const int OrePerBody = 10;

        // 最大身体数
        public const int MaxBodySegments = 30;

        public const float MaxSpeed = 8f;
        public const float Acceleration = 0.20f;
        public const float TurnSpeed = 0.055f;
        public const float BrakeAmount = 0.18f;
        public const float SegmentDistance = 36f;
        public const int UndergroundMargin = 10;

        public bool PinkyWormActive;

        public bool IsMoving =>
            PinkyWormActive &&
            WormSpeed > 0.15f;

        public float WormSpeed;
        public float WormRotation;

        public int CollectedOre;
        public int BodySegments;

        // =========================================================
        // Worm position
        // =========================================================

        private Vector2 wormPosition;

        // =========================================================
        // Position history
        // =========================================================

        private readonly List<Vector2> positionHistory =
            new List<Vector2>();

        public IReadOnlyList<Vector2> PositionHistory =>
            positionHistory;

        // =========================================================
        // NPC hit cooldown
        // =========================================================

        private readonly Dictionary<int, int> hitCooldowns =
            new Dictionary<int, int>();

        // =========================================================
        // Worm sound
        // =========================================================

        private int wormSoundTimer;

        public const int MinWormSoundInterval = 8;
        public const int MaxWormSoundInterval = 35;

        // =========================================================
        // Initialize
        // =========================================================

        public override void Initialize()
        {
            PinkyWormActive = false;

            WormSpeed = 0f;
            WormRotation = 0f;

            CollectedOre = 0;
            BodySegments = 0;

            wormPosition = Vector2.Zero;

            positionHistory.Clear();
            hitCooldowns.Clear();

            wormSoundTimer = 0;
        }

        // =========================================================
        // Start
        // =========================================================

        public void StartWorm()
        {
            PinkyWormActive = true;

            WormSpeed = 0f;

            BodySegments = 0;
            CollectedOre = 0;

            positionHistory.Clear();

            WormRotation =
                Player.direction == -1
                    ? MathF.PI
                    : 0f;

            Vector2 center =
                Player.Center;

            // ワーム専用位置
            wormPosition =
                Player.position;

            for (int i = 0; i < 300; i++)
                positionHistory.Add(center);

            hitCooldowns.Clear();

            wormSoundTimer = 0;

            Player.width = 32;
            Player.height = 32;

            Player.noFallDmg = true;
            Player.velocity = Vector2.Zero;
            Player.gravity = 0f;
        }

        // =========================================================
        // Stop
        // =========================================================

        public void StopWorm()
        {
            PinkyWormActive = false;

            WormSpeed = 0f;
            CollectedOre = 0;
            BodySegments = 0;

            positionHistory.Clear();
            hitCooldowns.Clear();

            wormSoundTimer = 0;

            Player.velocity = Vector2.Zero;
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

            // =====================================================
            // ワーム本来の位置を維持
            // =====================================================

            Player.position =
                wormPosition;

            Player.gravity = 0f;
            Player.velocity = Vector2.Zero;
            Player.noFallDmg = true;

            UpdateHitCooldowns();
        }

        // =========================================================
        // PreUpdateMovement
        // =========================================================

        public override void PreUpdateMovement()
        {
            if (!PinkyWormActive)
                return;

            Player.velocity = Vector2.Zero;
            Player.gravity = 0f;
            Player.oldVelocity = Vector2.Zero;
        }

        // =========================================================
        // PostUpdate
        // =========================================================

        public override void PostUpdate()
        {
            if (!PinkyWormActive)
                return;

            if (Player.dead)
                return;

            Player.velocity = Vector2.Zero;
            Player.gravity = 0f;

            float surfaceY =
                (float)(Main.worldSurface * 16.0);

            if (Player.Center.Y <
                surfaceY +
                UndergroundMargin * 16f)
            {
                WormSpeed = 0f;

                Vector2 correctedCenter =
                    Player.Center;

                correctedCenter.Y =
                    surfaceY +
                    UndergroundMargin * 16f;

                Player.Center =
                    correctedCenter;

                wormPosition =
                    Player.position;

                return;
            }

            // =====================================================
            // 左右旋回
            // =====================================================

            if (Player.controlLeft)
                WormRotation -= TurnSpeed;

            if (Player.controlRight)
                WormRotation += TurnSpeed;

            // =====================================================
            // 加速
            // =====================================================

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

            // =====================================================
            // 減速
            // =====================================================

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

            // =====================================================
            // 自然減速
            // =====================================================

            if (!Player.controlUp &&
                !Player.controlDown)
            {
                WormSpeed *= 0.985f;

                if (WormSpeed < 0.05f)
                    WormSpeed = 0f;
            }

            // =====================================================
            // 移動方向
            // =====================================================

            Vector2 direction =
                new Vector2(
                    MathF.Cos(WormRotation),
                    MathF.Sin(WormRotation)
                );

            Vector2 movement =
                direction *
                WormSpeed;

            // =====================================================
            // ワーム専用座標を移動
            // =====================================================

            wormPosition +=
                movement;

            // =====================================================
            // Player位置をワーム座標に強制
            // =====================================================

            Player.position =
                wormPosition;

            Player.velocity =
                Vector2.Zero;

            Player.direction =
                MathF.Cos(WormRotation) >= 0f
                    ? 1
                    : -1;

            // =====================================================
            // 履歴
            // =====================================================

            UpdatePositionHistory(
                Player.Center
            );

            // =====================================================
            // 採掘
            // =====================================================

            TryMineOre();

            // =====================================================
            // 攻撃
            // =====================================================

            TryAttackNPCs();

            // =====================================================
            // 音
            // =====================================================

            UpdateWormSound();
        }

        // =========================================================
        // Draw layers
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
                if (layer == wormLayer)
                    continue;

                layer.Hide();
            }
        }

        // =========================================================
        // Position history
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
        // Head hitbox
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
        // NPC attack
        // =========================================================

        private void TryAttackNPCs()
        {
            Rectangle head =
                GetHeadHitbox();

            // =====================================================
            // 攻撃力計算
            //
            // 基本攻撃力20
            // 身体1節につき+10
            //
            // 例：
            // 0節 = 20
            // 1節 = 30
            // 2節 = 40
            // 10節 = 120
            // 30節 = 320
            // =====================================================

            int damage =
                HitDamage +
                BodySegments *
                DamagePerBodySegment;

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

                if (hitCooldowns.TryGetValue(
                        i,
                        out int cooldown) &&
                    cooldown > 0)
                    continue;

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

                Vector2 direction =
                    new Vector2(
                        MathF.Cos(WormRotation),
                        MathF.Sin(WormRotation)
                    );

                int hitDirection =
                    direction.X >= 0f
                        ? 1
                        : -1;

                NPC.HitInfo hit =
                    new NPC.HitInfo
                    {
                        Damage = damage,
                        Knockback = knockback,
                        HitDirection =
                            hitDirection,
                        Crit = false
                    };

                npc.StrikeNPC(
                    hit,
                    false,
                    false
                );

                npc.velocity +=
                    direction *
                    knockback;

                hitCooldowns[i] =
                    HitCooldown;

                for (int d = 0;
                     d < 8;
                     d++)
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

                SoundEngine.PlaySound(
                    SoundID.NPCHit1,
                    npc.Center
                );
            }
        }

        // =========================================================
        // Hit cooldown
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
                    remove.Add(pair.Key);
                else
                    hitCooldowns[pair.Key] =
                        value;
            }

            foreach (int id in remove)
                hitCooldowns.Remove(id);
        }

        // =========================================================
        // Ore mining
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
                        continue;

                    Tile tile =
                        Main.tile[x, y];

                    if (tile == null ||
                        !tile.HasTile)
                        continue;

                    if (!TileID.Sets.Ore[
                            tile.TileType])
                        continue;

                    Rectangle tileRect =
                        new Rectangle(
                            x * 16,
                            y * 16,
                            16,
                            16
                        );

                    if (!head.Intersects(
                            tileRect))
                        continue;

                    WorldGen.KillTile(
                        x,
                        y
                    );
                }
            }
        }

        // =========================================================
        // Ore collection
        // =========================================================

        public void AddCollectedOre(
            int amount)
        {
            if (!PinkyWormActive)
                return;

            if (amount <= 0)
                return;

            CollectedOre +=
                amount;

            while (CollectedOre >=
                   OrePerBody)
            {
                CollectedOre -=
                    OrePerBody;

                if (BodySegments <
                    MaxBodySegments)
                {
                    BodySegments++;

                    SpawnGrowthEffect();
                }
            }
        }

        // =========================================================
        // Growth effect
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

            SoundEngine.PlaySound(
                SoundID.ResearchComplete,
                Player.Center
            );
        }

        // =========================================================
        // Worm sound
        // =========================================================

        private void UpdateWormSound()
        {
            if (!PinkyWormActive)
                return;

            if (WormSpeed <= 0.15f)
            {
                wormSoundTimer = 0;
                return;
            }

            if (wormSoundTimer > 0)
            {
                wormSoundTimer--;
                return;
            }

            float speedRatio =
                MathHelper.Clamp(
                    WormSpeed / MaxSpeed,
                    0f,
                    1f
                );

            int interval =
                (int)MathHelper.Lerp(
                    MaxWormSoundInterval,
                    MinWormSoundInterval,
                    speedRatio
                );

            float pitch =
                MathHelper.Lerp(
                    -0.15f,
                    0.10f,
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

            wormSoundTimer =
                interval;
        }

        // =========================================================
        // Item use restriction
        // =========================================================

        public override bool CanUseItem(
            Item item)
        {
            if (PinkyWormActive)
            {
                if (item.damage > 0)
                    return false;
            }

            return true;
        }
    }
}