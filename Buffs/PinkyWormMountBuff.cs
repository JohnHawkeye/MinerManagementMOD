using Terraria;
using Terraria.ModLoader;
using MinerManagementMOD.Common.Players;

namespace MinerManagementMOD.Buffs
{
    public class PinkyWormMountBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(
            Player player,
            ref int buffIndex)
        {
            player.GetModPlayer<PinkyWormPlayer>()
                .PinkyWormActive = true;
        }
    }
}