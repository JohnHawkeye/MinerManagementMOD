using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace MinerManagementMOD.NPCs
{
    [AutoloadHead]
    public class GuardNPC : ModNPC
    {
        private const float FollowSpeed = 3f;

        public enum GuardState
        {
            Following,
            MovingToEnemy,
            Attacking,
            KnockedOut
        }

        public GuardState CurrentState = GuardState.Following;

        private NPC targetEnemy;
        private const float SearchRange = 80f;
        private const float MaxFollowRange = 250f;

        private const float AttackDistance = 32f;
        private const int AttackInterval = 25; // 約0.4秒
        private const int AttackDamage = 15;
        private const float AttackKnockback = 4f;

        private Item weapon;
        private bool isSwingingSword = false;
        private int swingFrame = 0;
        private int swordFrameTimer = 0;
        private int attackTimer = 0;
        private bool hasHitThisSwing = false;

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
            weapon.SetDefaults(ItemID.CopperBroadsword);
        }

        public override void AI()
        {
            // デスポーン防止
            NPC.timeLeft = 60;

            NPC.TargetClosest();

            Player player = Main.player[NPC.target];

            if (!player.active || player.dead)
                return;

            float distanceToPlayer = Vector2.Distance(NPC.Center, player.Center);
            if (distanceToPlayer > 600f)
            {
                NPC.Center = player.Center;
                // 速度をリセット 
                NPC.velocity = Vector2.Zero;
                // ワープ音 
                SoundEngine.PlaySound(SoundID.Item8, player.Center);
                // マルチプレイ時にも位置を同期 
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
                float dir = player.Center.X > NPC.Center.X ? 1f : -1f;

                NPC.velocity.X += (dir * FollowSpeed - NPC.velocity.X) * 0.15f;

                NPC.direction = dir > 0 ? 1 : -1;
                NPC.spriteDirection = NPC.direction;
            }
            else
            {
                NPC.velocity.X *= 0.85f;
            }

            if (player.Center.Y + 32 < NPC.Center.Y &&
                NPC.collideY)
            {
                NPC.velocity.Y = -7f;
            }
        }

        private void MoveToEnemy(Player player)
        {
            // プレイヤーから離れすぎたら帰る
            if (Vector2.Distance(NPC.Center, player.Center) > MaxFollowRange)
            {
                targetEnemy = null;
                CancelAttack();
                CurrentState = GuardState.Following;
                return;
            }

            // 敵がいなくなった
            if (targetEnemy == null || !targetEnemy.active || targetEnemy.life <= 0)
            {
                targetEnemy = null;
                CancelAttack();
                CurrentState = GuardState.Following;
                return;
            }

            float enemyDistance =
                Vector2.Distance(NPC.Center, targetEnemy.Center);

            if (enemyDistance > SearchRange)
            {
                targetEnemy = null;
                CancelAttack();
                CurrentState = GuardState.Following;
                return;
            }

            // 攻撃距離まで近付いた
            if (enemyDistance <= AttackDistance)
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
                targetEnemy = null;
                CancelAttack();
                CurrentState = GuardState.Following;
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

            attackTimer++;

            if (!isSwingingSword &&
                attackTimer >= weapon.useTime)
            {
                attackTimer = 0;

                // 向きを固定
                NPC.direction =
                    targetEnemy.Center.X > NPC.Center.X ? 1 : -1;

                NPC.spriteDirection = NPC.direction;

                isSwingingSword = true;
                swingFrame = 0;
                swordFrameTimer = 0;
                hasHitThisSwing = false;
            }

            if (!isSwingingSword)
                return;

            swordFrameTimer++;

            if (swordFrameTimer >= 5)
            {
                swordFrameTimer = 0;

                swingFrame++;

                if (swingFrame == 2 &&
                    !hasHitThisSwing)
                {
                    DamageEnemy();
                    hasHitThisSwing = true;
                }

                if (swingFrame >= 5)
                {
                    CancelAttack();
                }
            }
        }

        private void DamageEnemy()
        {
            if (targetEnemy == null)
                return;

            Rectangle swordHitbox;

            if (NPC.direction == 1)
            {
                swordHitbox = new Rectangle(
                    (int)NPC.Center.X,
                    (int)NPC.Center.Y - 18,
                    36,
                    36);
            }
            else
            {
                swordHitbox = new Rectangle(
                    (int)NPC.Center.X - 36,
                    (int)NPC.Center.Y - 18,
                    36,
                    36);
            }

            if (!swordHitbox.Intersects(targetEnemy.Hitbox))
                return;

            targetEnemy.StrikeNPC(
                new NPC.HitInfo()
                {
                    Damage = weapon.damage,
                    Knockback = weapon.knockBack,
                    HitDirection = NPC.direction
                });
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
        private void CancelAttack()
        {
            isSwingingSword = false;
            swingFrame = 0;
            swordFrameTimer = 0;
            attackTimer = 0;
            hasHitThisSwing = false;

            NPC.frameCounter = 0;
            NPC.frame.Y = 0;
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
                NPC.frameCounter = 0;
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
            if (System.Math.Abs(NPC.velocity.X) > 0.1f)
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
                        NPC.frame.Y = 0;
                }
            }
        }

        public override void PostDraw(
            SpriteBatch spriteBatch,
            Vector2 screenPos,
            Color drawColor)
        {
            DrawLeadArmor(spriteBatch, screenPos);
            DrawSword(spriteBatch, screenPos);
        }

        private void DrawSword(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            if (!isSwingingSword)
                return;

            Texture2D texture =
                TextureAssets.Item[weapon.type].Value;

            SpriteEffects effects =
                NPC.spriteDirection == -1 ?
                SpriteEffects.FlipHorizontally :
                SpriteEffects.None;

            Vector2 handOffset =
                new Vector2(8f * NPC.direction, -12f);

            Vector2 drawPos =
                NPC.Center + handOffset - screenPos;

            Vector2 origin =
                new Vector2(texture.Width * 0.15f,
                            texture.Height * 0.9f);

            float rotation;
            float[] RightSwing =
             {
                -135f,
                -100f,
                -45f,
                10f,
                70f
            };

            float[] LeftSwing =
            {
                -45f,
                -80f,
                -135f,
                -190f,
                -250f
            };

            if (NPC.direction == 1)
            {
                rotation =
                    MathHelper.ToRadians(RightSwing[swingFrame]);
            }
            else
            {
                rotation =
                    MathHelper.ToRadians(LeftSwing[swingFrame]);
            }

            spriteBatch.Draw(
                texture,
                drawPos,
                null,
                Lighting.GetColor(
                    (int)NPC.Center.X / 16,
                    (int)NPC.Center.Y / 16),
                rotation,
                origin,
                1f,
                effects,
                0f);
        }
        private void DrawLeadArmor(
            SpriteBatch spriteBatch,
            Vector2 screenPos)
        {
            Main.instance.LoadArmorHead(
                ArmorIDs.Head.LeadHelmet);

            Main.instance.LoadArmorBody(
                ArmorIDs.Body.LeadChainmail);

            Main.instance.LoadArmorLegs(
                ArmorIDs.Legs.LeadGreaves);

            Texture2D helmet =
                TextureAssets.ArmorHead[
                    ArmorIDs.Head.LeadHelmet
                ].Value;

            Texture2D body =
                TextureAssets.ArmorBody[
                    ArmorIDs.Body.LeadChainmail
                ].Value;

            Texture2D legs =
                TextureAssets.ArmorLeg[
                    ArmorIDs.Legs.LeadGreaves
                ].Value;

            SpriteEffects effects =
                NPC.spriteDirection == -1
                    ? SpriteEffects.FlipHorizontally
                    : SpriteEffects.None;

            Color color =
                Lighting.GetColor(
                    (int)NPC.Center.X / 16,
                    (int)NPC.Center.Y / 16);

            // 胴
            spriteBatch.Draw(
                body,
                NPC.Center - screenPos + new Vector2(0f, 2f),
                null,
                color,
                0f,
                new Vector2(body.Width / 2f, body.Height / 2f),
                1f,
                effects,
                0f);

            // 脚
            spriteBatch.Draw(
                legs,
                NPC.Center - screenPos + new Vector2(0f, 14f),
                null,
                color,
                0f,
                new Vector2(legs.Width / 2f, legs.Height / 2f),
                1f,
                effects,
                0f);

            // 頭
            spriteBatch.Draw(
                helmet,
                NPC.Center - screenPos + new Vector2(0f, -15f),
                null,
                color,
                0f,
                new Vector2(helmet.Width / 2f, helmet.Height / 2f),
                1f,
                effects,
                0f);
        }
    }
}