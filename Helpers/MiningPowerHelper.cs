using Terraria;

namespace MinerManagementMOD.Helpers
{
    public static class MiningPowerHelper
    {
        /// <summary>
        /// バフ・装備込みの採掘力を取得
        /// </summary>
        public static float GetMiningPower(Player player, Item item)
        {
            if (item.pick <= 0)
                return 0;

            float power = item.pick;

            // 採掘ポーション
            if (player.pickSpeed < 1f)
                power *= 1.25f;

            // TODO: 鉱夫装備
            // power *= 1.20f;

            // TODO: アクセサリ
            // power *= player.GetModPlayer<MMMPlayer>().MiningPowerMultiplier;

            return power;
        }

        public static int GetMiningDamage(Player player, Item item)
        {
            float power = GetMiningPower(player, item);

            return (int)(power * 0.30f);
        }
    }
}