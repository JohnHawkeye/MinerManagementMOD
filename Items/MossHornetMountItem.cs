using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using MinerManagementMOD.Mounts;

namespace MinerManagementMOD.Items
{
    public class MossHornetMountItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.autoReuse = false;

            Item.value = Item.buyPrice(gold: 1);
            Item.rare = ItemRarityID.Lime;

            // モスホーネットマウント
            Item.mountType = ModContent.MountType<MossHornetMount>();
        }
    }
}