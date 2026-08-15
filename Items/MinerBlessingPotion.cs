using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MinerManagementMOD.Items
{
    public class MinerBlessingPotion : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;

            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.useAnimation = 15;
            Item.useTime = 15;

            Item.useTurn = true;
            Item.UseSound = SoundID.Item3;

            Item.maxStack = 999;
            Item.consumable = true;

            Item.value = Item.buyPrice(gold: 50);
            Item.rare = ItemRarityID.Blue;

            Item.buffType = BuffID.Mining;
            Item.buffTime = 60 * 60 * 5;
        }

        public override bool? UseItem(Player player)
        {
            player.AddBuff(BuffID.Builder, 60 * 60 * 5);
            player.AddBuff(BuffID.Mining, 60 * 60 * 5);
            player.AddBuff(BuffID.Calm, 60 * 60 * 5);
            player.AddBuff(BuffID.Ironskin, 60 * 60 * 5);
            player.AddBuff(BuffID.Endurance, 60 * 60 * 5);
            player.AddBuff(BuffID.ObsidianSkin, 60 * 60 * 5);
            player.AddBuff(BuffID.Spelunker, 60 * 60 * 5);
            player.AddBuff(BuffID.Shine, 60 * 60 * 5);
            player.AddBuff(BuffID.NightOwl, 60 * 60 * 5);
            player.AddBuff(BuffID.Dangersense, 60 * 60 * 5);

            return true;
        }
    }
}