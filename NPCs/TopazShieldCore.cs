using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace MinerManagementMOD.NPCs
{
    public class TopazShieldCore : ModNPC
    {
        public override void SetDefaults()
        {
            NPC.width = 40;
            NPC.height = 40;

            NPC.lifeMax = 200;

            NPC.defense = 20;

            NPC.noGravity = true;
            NPC.noTileCollide = true;

            NPC.knockBackResist = 0f;

            NPC.dontTakeDamage = false;
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

            NPC.velocity = Vector2.Zero;

            Lighting.AddLight(
                NPC.Center,
                1.0f,
                0.7f,
                0.2f);

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
    }
}