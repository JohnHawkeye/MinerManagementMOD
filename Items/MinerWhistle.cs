using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using MinerManagementMOD.NPCs;

namespace MinerManagementMOD.Items
{
    public class MinerWhistle : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;

            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTime = 20;
            Item.useAnimation = 20;

            Item.UseSound = SoundID.Item37;

            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(silver: 50);

            Item.consumable = false;
        }

        public override bool? UseItem(Player player)
        {
            foreach (NPC npc in Main.npc)
            {
                if (!npc.active)
                    continue;

                if (npc.type != ModContent.NPCType<MinerNPC>())
                    continue;

                Vector2 spawnPos = new Vector2(
                    player.Center.X + player.direction *24f,
                    player.Bottom.Y - npc.height
                ); 

                npc.Teleport(spawnPos, TeleportationStyleID.RodOfDiscord);
                npc.velocity = Vector2.Zero;

                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.SyncNPC, number: npc.whoAmI);
            }

            return true;
        }
        
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Wood, 1)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}