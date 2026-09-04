using Terraria;
using Terraria.ModLoader;
using MinerManagementMOD.Systems;

namespace MinerManagementMOD.Buffs
{
    public class RainbowBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
        }

        public override void Update(
            Player player,
            ref int buffIndex)
        {
            RainbowBuffPlayer modPlayer =
                player.GetModPlayer<RainbowBuffPlayer>();

        }
    }
}