using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace MinerManagementMOD.NPCs
{
    public class TopazShieldCore : ModNPC
    {
        public override void SetDefaults()
        {
            NPC.width = 40;
            NPC.height = 40;

            NPC.defense = 20;
            NPC.lifeMax = 200;

            NPC.noGravity = true;
            NPC.noTileCollide = true;

            NPC.knockBackResist = 0f;

            NPC.dontTakeDamage = false;

            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPC.netAlways = true;
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override bool CheckDead()
        {
            PlayBreakEffect();

            return true;
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            // 飛び道具は通常ダメージ
        }

        public override void ModifyHitByItem(Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            if (item.pick > 0)
            {
                // ピックは採掘力でダメージ計算
            }
            // それ以外の近接武器は半減
            else
            {
                modifiers.FinalDamage *= 0.5f;
            }
        }

        public override bool PreDraw(
            SpriteBatch spriteBatch,
            Vector2 screenPos,
            Color drawColor)
        {
            Texture2D texture =
                TextureAssets.Npc[Type].Value;


            spriteBatch.Draw(
                texture,
                NPC.Center - screenPos,
                null,
                Color.White,
                0f,
                texture.Size() / 2,
                1f,
                SpriteEffects.None,
                0);


            return false;
        }

        public override void AI()
        {
            int bossIndex = (int)NPC.ai[0];


            if (bossIndex < 0 ||
                bossIndex >= Main.maxNPCs)
            {
                NPC.active = false;
                return;
            }

            NPC boss = Main.npc[bossIndex];

            if (!boss.active ||
                boss.type != ModContent.NPCType<TopazCrab>())
            {
                NPC.active = false;
                return;
            }

            NPC.velocity = Vector2.Zero;

            Lighting.AddLight(
                NPC.Center,
                1.0f,
                0.7f,
                0.2f);

            NPC.timeLeft = 60;
        }


        public override void OnKill()
        {
            int bossIndex = (int)NPC.ai[0];


            if (bossIndex < 0 ||
                bossIndex >= Main.maxNPCs)
            {
                return;
            }


            if (Main.npc[bossIndex].ModNPC is TopazCrab crab)
            {
                crab.CoreDestroyed();
            }
        }

        private void PlayBreakEffect()
        {
            // ガラスが割れる音
            SoundEngine.PlaySound(
                SoundID.Shatter,
                NPC.Center
            );


            // トパーズの破片
            for (int i = 0; i < 35; i++)
            {
                Vector2 velocity =
                    Main.rand.NextVector2Circular(6f, 6f);


                Dust dust =
                    Dust.NewDustPerfect(
                        NPC.Center,
                        DustID.GemTopaz,
                        velocity,
                        100,
                        Color.White,
                        1.8f
                    );

                dust.noGravity = true;
            }


            // 白い光の粒
            for (int i = 0; i < 15; i++)
            {
                Vector2 velocity =
                    Main.rand.NextVector2Circular(3f, 3f);


                Dust dust =
                    Dust.NewDustPerfect(
                        NPC.Center,
                        DustID.GemDiamond,
                        velocity,
                        100,
                        Color.White,
                        1.2f
                    );

                dust.noGravity = true;
            }


            // 少し光らせる
            for (int i = 0; i < 5; i++)
            {
                Lighting.AddLight(
                    NPC.Center,
                    1f,
                    0.8f,
                    0.3f
                );
            }
        }
    }
}