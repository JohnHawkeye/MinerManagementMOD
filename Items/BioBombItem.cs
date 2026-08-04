using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace MinerManagementMOD.Items
{
    public class BioBombItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.UseSound = SoundID.Item19;

            Item.noUseGraphic = true;
            Item.noMelee = true;

            Item.consumable = true;
            Item.maxStack = 999;

            Item.shoot = ModContent.ProjectileType<Projectiles.BioBombProjectile>();
            Item.shootSpeed = 8f;

            Item.value = Item.buyPrice(0, 0, 10);
            Item.rare = ItemRarityID.Green;
        }

        public override bool CanUseItem(Player player)
        {
            
            return true;
        }
    }
}