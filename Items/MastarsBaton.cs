using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using MinerManagementMOD.Systems;

namespace MinerManagementMOD.Items
{
    public class MastarsBaton : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;

            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;

            Item.consumable = false;

            Item.UseSound = SoundID.Item20;

            Item.rare = ItemRarityID.Orange;

            Item.value = Item.buyPrice(gold: 1);

            Item.damage = 0;
            Item.knockBack = 0f;
            Item.DamageType = DamageClass.Generic;
        }

        public override bool? UseItem(Player player)
        {
            // ON / OFFを切り替える
            MinerSystem.ToggleSpecialNPC();

            if (MinerSystem.SpecialNPCEnabled)
            {
                Main.NewText(
                    "ガード・ハンター・ヒーラーを召喚します。",
                    Color.LightGreen
                );

                SoundEngine.PlaySound(
                    SoundID.Item4,
                    player.Center
                );
            }
            else
            {
                Main.NewText(
                    "ガード・ハンター・ヒーラーの召喚を停止しました。",
                    Color.OrangeRed
                );

                SoundEngine.PlaySound(
                    SoundID.Item8,
                    player.Center
                );
            }

            return true;
        }
        
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.IronBar, 1)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}