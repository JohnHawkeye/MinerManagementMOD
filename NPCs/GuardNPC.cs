using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using System;
using ReLogic.Content;
using System.Linq;

namespace MinerManagementMOD.NPCs
{
    [AutoloadHead]
    public class GuardNPC : ModNPC
    {
        public enum GuardState
        {
            Following,
            MovingToEnemy,
            Attacking,
            KnockedOut
        }

        private const float FollowSpeed = 3f;
        private const float SearchRange = 800f;
        private const float MaxFollowRange = 250f;
        private const float AttackDistance = 56f;

        public GuardState CurrentState = GuardState.Following;

        private NPC targetEnemy;
        private Item weapon;

        private bool isSwingingSword = false;
        private int attackTimer = 0;
        private int swingFrame = 0;
        private int frameTimer;

        //
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 26;
            NPC.aiStyle = -1;
        }

        public override void SetDefaults()
        {
            NPC.width = 18;
            NPC.height = 40;

            NPC.damage = 0;
            NPC.defense = 15;
            NPC.lifeMax = 300;

            NPC.knockBackResist = 0.3f;

            NPC.friendly = true;
            NPC.townNPC = false;

            NPC.noGravity = false;
            NPC.noTileCollide = false;

            NPC.aiStyle = -1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            weapon = new Item();
            weapon.SetDefaults(ItemID.BreakerBlade);
            CancelAttack();
        }

        public override void AI()
        {
            // デスポーン防止
            NPC.timeLeft = 60;

            NPC.TargetClosest();

            Player player = Main.player[NPC.target];

            if (!player.active || player.dead)
                return;

            float playerDistance =
                Vector2.Distance(
                    NPC.Center,
                    player.Center);

            if (playerDistance > 600f)
            {
                NPC.Center = player.Center;
                NPC.velocity = Vector2.Zero;

                SoundEngine.PlaySound(SoundID.Item8, player.Center);

                NPC.netUpdate = true;

                targetEnemy = null;
                CancelAttack();

                CurrentState = GuardState.Following;
                return;
            }

            switch (CurrentState)
            {
                case GuardState.Following:
                    FollowPlayer(player);

                    targetEnemy = FindEnemy();

                    if (targetEnemy != null)
                    {
                        CurrentState = GuardState.MovingToEnemy;
                    }
                    break;

                case GuardState.MovingToEnemy:
                    MoveToEnemy(player);
                    break;

                case GuardState.Attacking:
                    AttackEnemy(player);
                    break;
            }
        }

        private void FollowPlayer(Player player)
        {
            float distance = Vector2.Distance(NPC.Center, player.Center);

            if (distance > 100f)
            {
                float direction = player.Center.X > NPC.Center.X ? 1f : -1f;

                NPC.velocity.X +=
                    (
                    direction * FollowSpeed
                    -
                    NPC.velocity.X
                    )
                    * 0.15f;

                NPC.direction = direction > 0 ? 1 : -1;
                NPC.spriteDirection = NPC.direction;
            }
            else
            {
                NPC.velocity.X *= 0.85f;
            }

            if (player.Center.Y + 24 < NPC.Center.Y &&
                NPC.collideY)
            {
                NPC.velocity.Y = -7f;
            }
        }

        private void MoveToEnemy(Player player)
        {
            if (targetEnemy == null ||
             !targetEnemy.active ||
             targetEnemy.life <= 0)
            {
                ResetTarget();
                return;
            }

            // プレイヤーから離れすぎたら帰る
            if (Vector2.Distance(NPC.Center, player.Center) > MaxFollowRange)
            {
                ResetTarget();
                return;
            }
            float distance =
                Vector2.Distance(NPC.Center, targetEnemy.Center);


            // 攻撃距離まで近付いた
            if (distance <= AttackDistance)
            {
                NPC.velocity.X = 0;
                CurrentState = GuardState.Attacking;
                return;
            }

            float dir =
                targetEnemy.Center.X > NPC.Center.X ? 1f : -1f;

            NPC.velocity.X +=
                (dir * FollowSpeed - NPC.velocity.X) * 0.15f;

            NPC.direction = dir > 0 ? 1 : -1;
            NPC.spriteDirection = NPC.direction;

            // ジャンプ
            if (targetEnemy.Center.Y + 24 < NPC.Center.Y &&
                NPC.collideY)
            {
                NPC.velocity.Y = -7f;
            }
        }

        private void AttackEnemy(Player player)
        {
            if (targetEnemy == null ||
                !targetEnemy.active ||
                targetEnemy.life <= 0)
            {
                ResetTarget();

                return;
            }

            float enemyDistance =
                Vector2.Distance(NPC.Center, targetEnemy.Center);

            // 攻撃していない時だけ追いかける
            if (!isSwingingSword &&
                enemyDistance > AttackDistance)
            {
                CurrentState = GuardState.MovingToEnemy;
                return;
            }

            NPC.velocity.X = 0;
            NPC.direction =
                targetEnemy.Center.X > NPC.Center.X ? 1 : -1;
            NPC.spriteDirection = NPC.direction;

            attackTimer++;

            if (!isSwingingSword &&
                attackTimer >= weapon.useTime)
            {
                attackTimer = 0;

                isSwingingSword = true;

                Projectile.NewProjectile(
                    NPC.GetSource_FromAI(),
                    NPC.Center,
                    Vector2.Zero,
                    ModContent.ProjectileType<Projectiles.GuardSwordProjectile>(),
                    weapon.damage,
                    weapon.knockBack,
                    Main.myPlayer,
                    NPC.whoAmI,
                    NPC.direction
                );

                NPC.netUpdate = true;
            }

            if (!isSwingingSword)
                return;

            frameTimer++;

            int frameDelay =
                Math.Max(2, weapon.useAnimation / 18);

            if (frameTimer >= frameDelay)
            {
                frameTimer = 0;
                swingFrame++;
            }

            if (!Main.projectile.Any(p =>
                p.active &&
                p.type == ModContent.ProjectileType<Projectiles.GuardSwordProjectile>() &&
                p.ai[0] == NPC.whoAmI))
            {
                isSwingingSword = false;
            }
        }
        private void ResetTarget()
        {
            targetEnemy = null;
            CancelAttack();
            CurrentState =
                GuardState.Following;
        }

                private void CancelAttack()
        {
            isSwingingSword = false;

            attackTimer = 0;
            frameTimer = 0;
            swingFrame = 0;

        }

        private NPC FindEnemy()
        {
            NPC nearest = null;
            float nearestDistance = SearchRange;

            foreach (NPC npc in Main.npc)
            {
                if (!npc.active)
                    continue;

                if (npc.friendly)
                    continue;

                if (npc.life <= 0)
                    continue;

                if (npc.dontTakeDamage)
                    continue;

                float distance = Vector2.Distance(NPC.Center, npc.Center);

                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearest = npc;
                }
            }

            return nearest;
        }



        public override bool CanChat()
        {
            return true;
        }

        public override string GetChat()
        {
            return "周囲の警戒は任せてください。";
        }

        public override void FindFrame(int frameHeight)
        {
            //武器攻撃
            if (isSwingingSword)
            {
                NPC.frame.Y = frameHeight * (17 + swingFrame);
                return;
            }

            // 空中
            if (!NPC.collideY)
            {
                NPC.frame.Y = frameHeight * 15;
                return;
            }

            // 歩行
            if (Math.Abs(NPC.velocity.X) > 0.1f)
            {
                NPC.frameCounter++;

                if (NPC.frameCounter >= 6)
                {
                    NPC.frameCounter = 0;

                    NPC.frame.Y += frameHeight;

                    if (NPC.frame.Y < frameHeight * 7 ||
                        NPC.frame.Y > frameHeight * 15)
                    {
                        NPC.frame.Y = frameHeight * 7;
                    }
                }
            }
            else
            {
                // 待機
                NPC.frameCounter++;

                if (NPC.frameCounter >= 12)
                {
                    NPC.frameCounter = 0;

                    NPC.frame.Y += frameHeight;

                    if (NPC.frame.Y > frameHeight * 7)
                    { NPC.frame.Y = 0; }
                }
            }
        }
    }
}