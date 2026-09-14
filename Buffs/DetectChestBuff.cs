using Terraria;
using Terraria.ModLoader;

namespace MinerManagementMOD.Buffs
{
    public class DetectChestBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(
            Player player,
            ref int buffIndex
        )
        {
            // 実質的に無期限。
            // プレイヤーがバフを右クリックして解除できる。
            player.buffTime[buffIndex] = 2;
        }
    }
}