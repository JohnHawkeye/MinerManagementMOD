using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MinerManagementMOD.Items
{
    public class TopazCrabStone: ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;

            Item.maxStack = 20;

            Item.useTime = 45;
            Item.useAnimation = 45;

            Item.useStyle = ItemUseStyleID.HoldUp;

            Item.consumable = false;

            Item.noMelee = true;

            Item.value = Item.buyPrice(silver: 50);

            Item.rare = ItemRarityID.Orange;
        }


        public override bool? UseItem(Player player)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                NPC.SpawnOnPlayer(
                    player.whoAmI,
                    ModContent.NPCType<NPCs.TopazCrab>()
                );
            }

            return true;
        }


        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Topaz, 10)
                .AddIngredient(ModContent.ItemType<CrabClaw>(), 5)
                .AddIngredient(ItemID.StoneBlock, 300)
                .AddIngredient(ItemID.DirtBlock, 300)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}