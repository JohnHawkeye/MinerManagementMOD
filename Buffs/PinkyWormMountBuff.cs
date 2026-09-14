using Terraria;
using Terraria.ModLoader;
using MinerManagementMOD.Common.Players;
using MinerManagementMOD.Mounts;

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
            var worm = player.GetModPlayer<PinkyWormPlayer>();
            worm.PinkyWormActive = true;

            player.buffTime[buffIndex] = 10;
        }
    }
}