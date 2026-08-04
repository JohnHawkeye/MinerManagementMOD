using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using System;
using System.Collections.Generic;

namespace MinerManagementMOD.NPCs
{
    [AutoloadHead]
    public class HealerNPC : ModNPC
    {
        public enum GuardState
        {
            Following,
        }

        private const float FollowSpeed = 3f;
        public GuardState CurrentState = GuardState.Following;

        private const int HealCooldown = 600;
        private const float HealThreshold = 0.7f;
        private const float HealRange = 300f;
        private int healTimer = 0;

        private bool isHealing = false;
        private int healAnimationTimer = 0;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 23;
            NPC.aiStyle = -1;
        }

        public override void SetDefaults()
        {
            NPC.width = 18;
            NPC.height = 40;

            NPC.damage = 0;
            NPC.defense = 8;
            NPC.lifeMax = 250;

            NPC.knockBackResist = 0.4f;

            NPC.friendly = true;
            NPC.townNPC = false;

            NPC.noGravity = false;
            NPC.noTileCollide = false;

            NPC.aiStyle = -1;
        }

        public override void AI()
        {
            NPC.timeLeft = 60;

            NPC.TargetClosest();

            Player player = Main.player[NPC.target];

            if (!player.active || player.dead)
                return;

            float distance = Vector2.Distance(NPC.Center, player.Center);

            if (distance > 600f)
            {
                NPC.Center = player.Center;
                NPC.velocity = Vector2.Zero;

                SoundEngine.PlaySound(SoundID.Item8, player.Center);

                NPC.netUpdate = true;
                return;
            }

            FollowPlayer(player);
            HealNearbyTarget(player);
        }

        private void FollowPlayer(Player player)
        {
            // プレイヤーの真後ろを目標位置にする
            Vector2 targetPos =
                player.Center +
                new Vector2(-player.direction * 48f, 0);

            Vector2 diff = targetPos - NPC.Center;

            // 横移動
            if (Math.Abs(diff.X) > 8f)
            {
                float dir = Math.Sign(diff.X);

                NPC.velocity.X +=
                    (dir * FollowSpeed - NPC.velocity.X) * 0.15f;
            }
            else
            {
                NPC.velocity.X *= 0.8f;
            }

            // 向きはプレイヤーと同じ
            NPC.direction = player.direction;
            NPC.spriteDirection = NPC.direction;

            // ジャンプ
            if (player.Center.Y + 24 < NPC.Center.Y &&
                NPC.collideY)
            {
                NPC.velocity.Y = -7f;
            }
        }

        private void HealNearbyTarget(Player player)
        {
            // クールタイム処理
            if (healTimer > 0)
            {
                healTimer--;
            }


            // 回復モーション解除
            if (healAnimationTimer > 0)
            {
                healAnimationTimer--;

                if (healAnimationTimer <= 0)
                {
                    isHealing = false;
                }
            }

            if (healTimer > 0)
                return;

            float distance =
                Vector2.Distance(
                    NPC.Center,
                    player.Center);

            // プレイヤー優先
            if (distance <= HealRange &&
               player.statLife < player.statLifeMax2 * HealThreshold)
            {
                HealPlayer(player);
                return;
            }


            // NPC回復
            foreach (NPC npc in Main.npc)
            {
                if (!npc.active)
                    continue;

                if (npc.whoAmI == NPC.whoAmI)
                    continue;

                // 味方NPCのみ
                if (!IsAllyNPC(npc))
                    continue;

                float npcDistance =
                    Vector2.Distance(
                        NPC.Center,
                        npc.Center);

                if (npcDistance > HealRange)
                    continue;

                if (npc.life < npc.lifeMax * HealThreshold)
                {
                    HealNPC(npc);
                    return;
                }
            }
        }
        private void HealPlayer(Player player)
        {

            int lostHealth =
                player.statLifeMax2 -
                player.statLife;


            if (lostHealth <= 0)
                return;


            // 回復


            player.statLife += lostHealth;

            if (player.statLife > player.statLifeMax2)
            {
                player.statLife =
                    player.statLifeMax2;
            }

            player.HealEffect(lostHealth);

            NPC.direction = player.direction;

            PlayHealEffect(player.Center);

            healTimer = HealCooldown;
        }

        private void HealNPC(NPC npc)
        {
            int lost =
                npc.lifeMax -
                npc.life;

            npc.life += lost;

            if (npc.life > npc.lifeMax)
                npc.life = npc.lifeMax;

            npc.HealEffect(lost);

            NPC.spriteDirection = NPC.direction;
            PlayHealEffect(npc.Center);
            StartHealCooldown();
        }

        private bool IsAllyNPC(NPC npc)
        {
            return
                npc.ModNPC is GuardNPC ||
                npc.ModNPC is HunterNPC ||
                npc.ModNPC is MinerNPC;
        }

        private void StartHealCooldown()
        {
            healTimer = HealCooldown;
        }

        public override bool CanChat()
        {
            return true;
        }

        public override string GetChat()
        {
            return "無理しちゃ駄目よん♪";
        }

        public override void FindFrame(int frameHeight)
        {
            if (isHealing)
            {
                NPC.frame.Y = frameHeight * 16;
                return;
            }

            // 空中
            if (!NPC.collideY)
            {
                NPC.frame.Y = frameHeight * 10;
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

                    if (NPC.frame.Y < frameHeight * 2 ||
                        NPC.frame.Y > frameHeight * 9)
                    {
                        NPC.frame.Y = frameHeight * 2;
                    }
                }
            }
            else
            {
                // 待機
                NPC.frame.Y = 0;
            }
        }

        private void PlayHealEffect(Vector2 position)
        {
            SoundEngine.PlaySound(
                SoundID.Item4,
                position);


            for (int i = 0; i < 20; i++)
            {
                Dust.NewDust(
                    position,
                    20,
                    20,
                    DustID.HealingPlus,
                    Main.rand.NextFloat(-2f, 2f),
                    Main.rand.NextFloat(-2f, 2f));
            }
            isHealing = true;
            healAnimationTimer = 60;
        }
    }
}