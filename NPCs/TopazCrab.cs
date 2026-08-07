using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using MinerManagementMOD.Projectiles;
using MinerManagementMOD.Helpers;
using Terraria.GameContent.ItemDropRules;
using MinerManagementMOD.Items;

namespace MinerManagementMOD.NPCs
{
    public class TopazCrab : ModNPC
    {
        public override string Texture => "MinerManagementMOD/Assets/NPCs/TopazCrab";

        private float leftClawOffsetY = -16f;
        private float rightClawOffsetY = -16f;
        private int attackSide = 0;

        private bool phase2 = false;
        private int lightShotCount;

        private bool phase3 = false;
        private bool shieldActive = false;
        private int shieldCoreCount = 0;

        private int shieldLaserTimer;

        private int[] coreIndex = new int[4];
        private bool shieldBreaking = false;
        private float shieldScale = 1f;
        private float shieldAlpha = 1f;
        private bool shieldBreakEffectPlayed = false;

        private bool breakEye = false;
        private bool deathEffectPlayed = false;
        private bool lastHitWasPickaxe;

        private enum BossState
        {
            SmallJump1,
            WaitSmall1,

            SmallJump2,
            WaitSmall2,

            BigJump,
            Air,

            WaitBig,

            CheckClaw,
            ClawPrepare,
            ClawSmash,
            ClawRecover,

            LightAttack,
            LightAttackWait,

            ShieldStart,
            ShieldMode,
            Break
        }

        private BossState State
        {
            get => (BossState)NPC.ai[0];
            set => NPC.ai[0] = (float)value;
        }

        private ref float Timer => ref NPC.ai[1];

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 1;
        }

        public override void SetDefaults()
        {
            NPC.width = 160;
            NPC.height = 128;

            NPC.damage = 24;
            NPC.defense = 12;
            NPC.lifeMax = 3000;

            NPC.knockBackResist = 0f;

            NPC.boss = true;

            NPC.noGravity = false;
            NPC.noTileCollide = false;

            NPC.aiStyle = -1;

            Music = MusicID.Boss1;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            // Topaz 30～50個
            npcLoot.Add(
                ItemDropRule.Common(
                    ItemID.Topaz,
                    1,
                    30,
                    50
                )
            );


            // カッパーマイナーコイン 100個固定
            npcLoot.Add(
                ItemDropRule.Common(
                    ModContent.ItemType<CopperMinerCoin>(),
                    1,
                    100,
                    100
                )
            );


            // トパーズストーンブロック 300個
            npcLoot.Add(
                ItemDropRule.Common(
                    ItemID.TopazStoneBlock,
                    1,
                    300,
                    300
                )
            );


            // ハート 2～5個
            npcLoot.Add(
                ItemDropRule.Common(
                    ItemID.Heart,
                    1,
                    2,
                    5
                )
            );
        }

        public override void ModifyHitByItem(Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            lastHitWasPickaxe = item.pick > 0;

            if (!lastHitWasPickaxe)
            {
                modifiers.SourceDamage *= 0f;
                return;
            }

            int damage =
                MiningPowerHelper.GetMiningDamage(player, item);

            modifiers.SetMaxDamage(damage);

        }

        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            // 飛び道具は無効
            modifiers.SetMaxDamage(1);
        }

        public override bool CheckDead()
        {
            if (!deathEffectPlayed)
            {
                deathEffectPlayed = true;
                PlayDeathEffect();
                SpawnDeathPieces();
            }
            return true;
        }

        public override void AI()
        {

            if (!phase2 &&
                NPC.life <= NPC.lifeMax * 0.7f)
            {
                StartPhase2();
            }

            if (!phase3 &&
                NPC.life <= NPC.lifeMax * 0.3f)
            {
                phase3 = true;

                Timer = 0;

                State = BossState.ShieldStart;
            }

            Player player = Main.player[NPC.target];

            NPC.TargetClosest();

            switch (State)
            {
                case BossState.SmallJump1:
                    SmallJump(player, BossState.WaitSmall1);
                    break;

                case BossState.WaitSmall1:
                    WaitSmall(BossState.SmallJump2);
                    break;

                case BossState.SmallJump2:
                    SmallJump(player, BossState.WaitSmall2);
                    break;

                case BossState.WaitSmall2:
                    WaitSmall(BossState.BigJump);
                    break;

                case BossState.BigJump:
                    BigJump(player);
                    break;

                case BossState.Air:
                    Air();
                    break;

                case BossState.WaitBig:
                    WaitBig();
                    break;

                case BossState.CheckClaw:
                    CheckClaw();
                    break;

                case BossState.ClawPrepare:
                    ClawPrepare();
                    break;

                case BossState.ClawSmash:
                    ClawSmash();
                    break;

                case BossState.ClawRecover:
                    ClawRecover();
                    break;
                case BossState.LightAttack:
                    LightAttack();
                    break;

                case BossState.ShieldStart:
                    ShieldStart();
                    break;

                case BossState.ShieldMode:
                    ShieldMode();
                    break;

                case BossState.Break:
                    BreakState();
                    break;
            }
        }

        private void SmallJump(Player player, BossState next)
        {
            if (NPC.velocity.Y == 0)
            {
                if (Timer == 0)
                {
                    NPC.direction =
                        player.Center.X > NPC.Center.X ? 1 : -1;

                    NPC.velocity.X = NPC.direction * 3f;
                    NPC.velocity.Y = -7f;

                    Timer = 1;
                }
            }

            if (Timer > 0 && NPC.velocity.Y == 0)
            {
                NPC.velocity.X = 0f;

                Timer = 0;
                State = next;

            }
        }

        private void WaitSmall(BossState next)
        {
            NPC.velocity.X = 0f;

            Timer++;

            if (Timer >= 60)
            {
                Timer = 0;
                State = next;
            }
        }

        private void BigJump(Player player)
        {
            if (NPC.velocity.Y == 0)
            {
                NPC.direction =
                    player.Center.X > NPC.Center.X ? 1 : -1;

                NPC.velocity.X = NPC.direction * 5f;
                NPC.velocity.Y = -7f;

                State = BossState.Air;
            }
        }

        private void WaitBig()
        {
            NPC.velocity.X = 0f;

            Timer++;

            if (Timer >= 120)
            {
                Timer = 0;

                if (phase2)
                {
                    lightShotCount = 0;
                    State = BossState.LightAttack;
                }
                else
                {
                    State = BossState.CheckClaw;
                }
            }
        }

        private void Air()
        {
            if (NPC.velocity.Y == 0)
            {
                NPC.velocity.X = 0f;

                Timer = 0;
                State = BossState.WaitBig;
            }
        }

        private void CheckClaw()
        {
            Player player = Main.player[NPC.target];

            float x = player.Center.X - NPC.Center.X;

            if (Math.Abs(x) < 120f)
            {
                if (x > 0)
                {
                    attackSide = 1;
                }
                else
                {
                    attackSide = -1;
                }
                State = BossState.ClawPrepare;
            }
            else
            {
                Timer = 0;
                State = BossState.SmallJump1;
            }
        }

        private void ClawPrepare()
        {
            Timer++;

            if (attackSide == 1)
            {
                rightClawOffsetY = -48f;
            }
            else
            {
                leftClawOffsetY = -48f;
            }

            if (Timer >= 60)
            {
                Timer = 0;
                State = BossState.ClawSmash;
            }
        }

        private void ClawSmash()
        {
            Timer++;

            if (Timer == 1)
            {
                if (attackSide == 1)
                {
                    rightClawOffsetY = 16f;
                }
                else
                {
                    leftClawOffsetY = 16f;
                }

                CreateClawDamage();
            }

            if (Timer >= 20)
            {
                Timer = 0;
                State = BossState.ClawRecover;
            }
        }

        private void ClawRecover()
        {
            Timer++;

            leftClawOffsetY = -16f;
            rightClawOffsetY = -16f;

            if (Timer >= 30)
            {
                Timer = 0;
                State = BossState.SmallJump1;
            }
        }

        private void CreateClawDamage()
        {
            Player player =
                Main.player[NPC.target];

            Rectangle hitbox;

            if (attackSide == 1)
            {
                hitbox = new Rectangle(
                    (int)NPC.Center.X + 60,
                    (int)NPC.Bottom.Y - 20,
                    80,
                    50);
            }
            else
            {
                hitbox = new Rectangle(
                    (int)NPC.Center.X - 140,
                    (int)NPC.Bottom.Y - 20,
                    80,
                    50);
            }

            if (hitbox.Intersects(player.Hitbox))
            {
                player.Hurt(
                    PlayerDeathReason.ByNPC(NPC.whoAmI),
                    40,
                    attackSide);
            }
        }

        private void LightAttack()
        {
            NPC.velocity.X = 0f;

            Timer++;

            if (Timer >= 60)
            {
                Timer = 0;

                FireLight();

                lightShotCount++;

                if (lightShotCount >= 4)
                {
                    State = BossState.CheckClaw;
                }
            }
        }

        private void FireLight()
        {
            Player player =
                Main.player[NPC.target];

            Vector2 velocity =
                player.Center - NPC.Center;

            velocity.Normalize();

            velocity *= 6f;

            SoundEngine.PlaySound(
                SoundID.Item33,
                NPC.Center);

            for (int i = 0; i < 10; i++)
            {
                Dust.NewDust(
                    NPC.Center,
                    4,
                    4,
                    DustID.GemTopaz);
            }

            Projectile.NewProjectile(
                NPC.GetSource_FromAI(),
                NPC.Center,
                velocity,
                ModContent.ProjectileType<TopazCrabLight>(),
                25,
                0f);
        }

        public override bool PreDraw(
            SpriteBatch spriteBatch,
            Vector2 screenPos,
            Color drawColor)
        {
            Texture2D body;
            Texture2D eye;
            Texture2D eyelid;
            Texture2D claw;

            if (phase2)
            {
                body = ModContent.Request<Texture2D>(
                    "MinerManagementMOD/Assets/NPCs/TopazCrab_body2").Value;

                if (breakEye)
                {
                    eye = ModContent.Request<Texture2D>(
                        "MinerManagementMOD/Assets/NPCs/TopazCrab_eye2").Value;
                }
                else
                {
                    eye = ModContent.Request<Texture2D>(
                        "MinerManagementMOD/Assets/NPCs/TopazCrab_eye").Value;
                }


                eyelid = ModContent.Request<Texture2D>(
                    "MinerManagementMOD/Assets/NPCs/TopazCrab_eyelid2").Value;

                claw = ModContent.Request<Texture2D>(
                    "MinerManagementMOD/Assets/NPCs/TopazCrab_claw2").Value;
            }
            else
            {

                body = TextureAssets.Npc[Type].Value;

                eye = ModContent.Request<Texture2D>(
                   "MinerManagementMOD/Assets/NPCs/TopazCrab_eye").Value;

                eyelid = ModContent.Request<Texture2D>(
                   "MinerManagementMOD/Assets/NPCs/TopazCrab_eyelid").Value;

                claw = ModContent.Request<Texture2D>(
                   "MinerManagementMOD/Assets/NPCs/TopazCrab_claw").Value;
            }

            Vector2 pos = NPC.Center - screenPos;

            spriteBatch.Draw(
                body,
                pos,
                null,
                Color.White,
                0f,
                body.Size() / 2,
                1f,
                SpriteEffects.None,
                0);

            DrawEyes(spriteBatch, eye, eyelid, screenPos);

            //drawing claw
            Vector2 rightClawPos = NPC.Center + new Vector2(96, rightClawOffsetY);
            Vector2 leftClawPos = NPC.Center + new Vector2(-96, leftClawOffsetY);

            spriteBatch.Draw(
                claw,
                rightClawPos - screenPos,
                null,
                Color.White,
                0f,
                claw.Size() / 2,
                1f,
                SpriteEffects.None,
                0);

            spriteBatch.Draw(
                claw,
                leftClawPos - screenPos,
                null,
                Color.White,
                0f,
                claw.Size() / 2,
                1f,
                SpriteEffects.FlipHorizontally,
                0);

            //draw shield
            if (shieldActive)
            {
                Texture2D shield =
                    ModContent.Request<Texture2D>(
                    "MinerManagementMOD/Assets/NPCs/TopazCrab_shield").Value;

                spriteBatch.Draw(
                    shield,
                    NPC.Center - screenPos,
                    null,
                    Color.White * shieldAlpha,
                    0f,
                    shield.Size() / 2,
                    shieldScale,
                    SpriteEffects.None,
                    0);
            }

            return false;
        }

        private void DrawEyes(
            SpriteBatch spriteBatch,
            Texture2D eye,
            Texture2D eyelid,
            Vector2 screenPos)
        {
            Player player = Main.player[NPC.target];

            Vector2 look =
                player.Center - NPC.Center;

            if (look != Vector2.Zero)
                look.Normalize();

            look *= 2f;

            Vector2 leftEye =
                NPC.Center +
                new Vector2(-32, -48);

            Vector2 rightEye =
                NPC.Center +
                new Vector2(32, -48);

            if (breakEye)
            {
                DrawBreakEye(
                    spriteBatch,
                    eye,
                    leftEye,
                    screenPos,
                    true);

                DrawBreakEye(
                    spriteBatch,
                    eye,
                    rightEye,
                    screenPos,
                    false);
            }
            else
            {
                DrawEye(spriteBatch, eye, leftEye, look, screenPos);
                DrawEye(spriteBatch, eye, rightEye, look, screenPos);
            }

            spriteBatch.Draw(
                eyelid,
                NPC.Center - screenPos,
                null,
                Color.White,
                0f,
                eyelid.Size() / 2,
                1f,
                SpriteEffects.None,
                0
            );
        }

        private void DrawEye(
            SpriteBatch spriteBatch,
            Texture2D eye,
            Vector2 worldPos,
            Vector2 look,
            Vector2 screenPos)
        {
            Vector2 pos = worldPos - screenPos;

            float rotation = look.ToRotation();

            spriteBatch.Draw(
                eye,
                pos,
                null,
                Color.White,
                rotation,
                eye.Size() / 2,
                1f,
                SpriteEffects.None,
                0);

        }

        private void DrawBreakEye(
            SpriteBatch spriteBatch,
            Texture2D eye,
            Vector2 worldPos,
            Vector2 screenPos,
            bool flip)
        {
            Vector2 pos = worldPos - screenPos;

            spriteBatch.Draw(
                eye,
                pos,
                null,
                Color.White,
                0f,
                eye.Size() / 2,
                1f,
                flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                0);
        }

        private void StartPhase2()
        {
            phase2 = true;

            SpawnShellPieces();

            SoundEngine.PlaySound(
                SoundID.NPCDeath14,
                NPC.Center);
        }

        private void SpawnShellPieces()
        {
            SpawnPiece(0, new Vector2(-6f, -8f));
            SpawnPiece(1, new Vector2(6f, -8f));
            SpawnPiece(2, new Vector2(-4f, -5f));
            SpawnPiece(3, new Vector2(4f, -5f));
        }

        private void SpawnPiece(int piece, Vector2 velocity)
        {
            int p = Projectile.NewProjectile(
                NPC.GetSource_FromAI(),
                NPC.Center,
                velocity,
                ModContent.ProjectileType<TopazCrabShell>(),
                0,
                0f);

            Main.projectile[p].ai[0] = piece;
        }

        private void ShieldStart()
        {
            shieldBreakEffectPlayed = false;

            shieldScale = 1f;
            shieldAlpha = 1f;

            shieldBreaking = false;

            NPC.velocity = Vector2.Zero;

            shieldActive = true;
            NPC.dontTakeDamage = true;

            SpawnShieldCores();

            Timer = 0;

            State = BossState.ShieldMode;
        }

        private void ShieldMode()
        {
            NPC.velocity = Vector2.Zero;

            if (shieldBreaking)
            {
                ShieldBreakAnimation();
                return;
            }

            shieldLaserTimer++;

            if (shieldLaserTimer >= Main.rand.Next(36, 90))
            {
                shieldLaserTimer = 0;

                FireShieldLaser();
            }
        }

        private void BreakState()
        {
            NPC.velocity = Vector2.Zero;
            breakEye = true;
            Timer++;

            if (Timer >= 360)
            {
                Timer = 0;
                breakEye = false;
                State = BossState.ShieldStart;
            }

        }

        private void SpawnShieldCores()
        {
            shieldCoreCount = 4;
            Vector2 center = NPC.Center;
            Vector2[] positions =
            {
                center + new Vector2(-300, -224),
                center + new Vector2(300, -224),

                center + new Vector2(-420, -96),
                center + new Vector2(420, -96)
            };

            for (int i = 0; i < positions.Length; i++)
            {
                int index =
                    NPC.NewNPC(
                        NPC.GetSource_FromAI(),
                        (int)positions[i].X,
                        (int)positions[i].Y,
                        ModContent.NPCType<TopazShieldCore>());

                Main.npc[index].ai[0] = NPC.whoAmI;
                Main.npc[index].netUpdate = true;

                coreIndex[i] = index;
            }
        }

        public void CoreDestroyed()
        {
            shieldCoreCount--;

            if (shieldCoreCount <= 0)
            {
                shieldBreaking = true;
                Timer = 0;
                NPC.dontTakeDamage = true;
            }
        }

        private void ShieldBreakAnimation()
        {
            Timer++;

            if (!shieldBreakEffectPlayed)
            {
                shieldBreakEffectPlayed = true;
                PlayShieldBreakEffect();
            }

            shieldScale += 0.015f;
            shieldAlpha -= 0.025f;

            if (shieldAlpha <= 0f)
            {
                shieldAlpha = 0f;

                shieldActive = false;
                shieldBreaking = false;

                NPC.dontTakeDamage = false;

                Timer = 0;
                RemoveShieldCores();
                State = BossState.Break;
            }
        }
        private void RemoveShieldCores()
        {
            for (int i = 0; i < 4; i++)
            {
                int index = coreIndex[i];

                if (index >= 0 &&
                   index < Main.maxNPCs)
                {
                    if (Main.npc[index].active)
                    {
                        Main.npc[index].active = false;
                    }
                }
            }
        }

        private void PlayShieldBreakEffect()
        {
            // ガラス破壊音
            SoundEngine.PlaySound(
                SoundID.Shatter,
                NPC.Center);



            // 光の破片
            for (int i = 0; i < 40; i++)
            {
                Vector2 velocity =
                    Main.rand.NextVector2Circular(8f, 8f);


                Dust dust =
                    Dust.NewDustPerfect(
                        NPC.Center,
                        DustID.GemTopaz,
                        velocity,
                        100,
                        Color.White,
                        1.8f);


                dust.noGravity = true;
            }


            // 追加の光粒
            for (int i = 0; i < 20; i++)
            {
                Vector2 velocity =
                    Main.rand.NextVector2Circular(4f, 4f);


                Dust dust =
                    Dust.NewDustPerfect(
                        NPC.Center,
                        DustID.GemDiamond,
                        velocity,
                        100,
                        Color.White,
                        1.2f);


                dust.noGravity = true;
            }
        }
        private void FireShieldLaser()
        {
            Player player =
                Main.player[NPC.target];

            Vector2 velocity =
                player.Center - NPC.Center;

            velocity.Normalize();

            velocity *= 6f;

            SoundEngine.PlaySound(
                SoundID.Item33,
                NPC.Center);

            Projectile.NewProjectile(
                NPC.GetSource_FromAI(),
                NPC.Center,
                velocity,
                ModContent.ProjectileType<TopazCrabLight>(),
                25,
                0f);
        }

        private void PlayDeathEffect()
        {
            // 砕ける音
            SoundEngine.PlaySound(
                SoundID.Shatter,
                NPC.Center);
            SoundEngine.PlaySound(
                SoundID.Item14,
                NPC.Center);

            // トパーズの光
            for (int i = 0; i < 100; i++)
            {
                Vector2 velocity =
                    Main.rand.NextVector2Circular(
                        10f,
                        10f);


                Dust dust =
                    Dust.NewDustPerfect(
                        NPC.Center,
                        DustID.GemTopaz,
                        velocity,
                        100,
                        Color.White,
                        2f);


                dust.noGravity = true;
            }


            // 白いきらめき
            for (int i = 0; i < 30; i++)
            {
                Vector2 velocity =
                    Main.rand.NextVector2Circular(
                        6f,
                        6f);


                Dust dust =
                    Dust.NewDustPerfect(
                        NPC.Center,
                        DustID.GemDiamond,
                        velocity,
                        100,
                        Color.White,
                        1.5f);


                dust.noGravity = true;
            }
        }
        private void SpawnDeathPieces()
        {
            Vector2[] velocities =
                {
                new Vector2(-6,-8),
                new Vector2(6,-8),
                new Vector2(-4,-5),
                new Vector2(4,-5)
            };


            for (int i = 0; i < 4; i++)
            {
                int p =
                    Projectile.NewProjectile(
                        NPC.GetSource_FromAI(),
                        NPC.Center,
                        velocities[i],
                        ModContent.ProjectileType<TopazCrabDeathPiece>(),
                        0,
                        0f);


                Main.projectile[p].ai[0] = i;
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (!lastHitWasPickaxe)
                return;

            // 死亡時は別処理にする
            if (NPC.life <= 0)
                return;


            // ツルハシ攻撃ではない場合は通常処理
            if (hit.Damage <= 0)
                return;


            // ==========================
            // フェーズ1
            // 土・石の採掘ダスト
            // ==========================

            if (NPC.life > NPC.lifeMax * 0.5f)
            {
                for (int i = 0; i < 5; i++)
                {
                    Dust.NewDust(
                        NPC.position,
                        NPC.width,
                        NPC.height,
                        DustID.Stone,
                        Main.rand.NextFloat(-2f, 2f),
                        Main.rand.NextFloat(-2f, 2f)
                    );
                }

                SoundEngine.PlaySound(
                    SoundID.Tink,
                    NPC.Center
                );
            }


            // ==========================
            // フェーズ2以降
            // トパーズの輝き
            // ==========================

            else
            {
                for (int i = 0; i < 8; i++)
                {
                    Dust dust = Dust.NewDustDirect(
                        NPC.position,
                        NPC.width,
                        NPC.height,
                        DustID.GemTopaz
                    );

                    dust.velocity *= 1.5f;
                    dust.noGravity = true;
                }


                SoundEngine.PlaySound(
                    SoundID.Item27,
                    NPC.Center
                );
            }
        }
    }
}