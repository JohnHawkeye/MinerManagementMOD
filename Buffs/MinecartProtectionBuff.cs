using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MinerManagementMOD.Buffs
{
    public class MinecartProtectionBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;

            Main.pvpBuff[Type] = false;
        }

        public override void Update(
            Player player,
            ref int buffIndex)
        {
            // トロッコから降りたら即座にバフを消す
            if (!player.mount.Active ||
                player.mount.Type != MountID.Minecart)
            {
                player.DelBuff(buffIndex);
                buffIndex--;
                return;
            }
        }
    }
}