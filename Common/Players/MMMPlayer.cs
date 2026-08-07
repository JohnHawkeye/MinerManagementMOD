using Terraria;
using Terraria.ModLoader;
using MinerManagementMOD.Helpers;

namespace MinerManagementMOD
{
    public class MMMPlayer : ModPlayer
    {
        public override void ModifyWeaponDamage(Item item, ref StatModifier damage)
        {
            // ピックアックスのみ
            if (item.pick <= 0)
                return;

            int miningDamage = MiningPowerHelper.GetMiningDamage(Player, item);

            // 武器攻撃力との差分を倍率化
            if (item.damage > 0)
            {
                float scale = (float)miningDamage / item.damage;
                damage *= scale;
            }
        }
    }
}