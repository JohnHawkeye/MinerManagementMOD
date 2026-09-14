using Terraria;
using Terraria.ModLoader;
using MinerManagementMOD.Items.Armor;

namespace MinerManagementMOD.Common.Players
{
    public class BlackSmithArmorPlayer : ModPlayer
    {
        // ブラックスミス装備のセット効果
        public bool BlackSmithSet;

        // 採掘範囲ボーナス
        public int BlackSmithMiningRangeBonus { get; private set; }

        public override void ResetEffects()
        {
            BlackSmithSet = false;
            BlackSmithMiningRangeBonus = 0;
        }

        public override void UpdateEquips()
        {
            // 頭・胴・脚の3部位を確認
            if (Player.armor[0].type == ModContent.ItemType<BlackSmithHat>() &&
                Player.armor[1].type == ModContent.ItemType<BlackSmithArmor>() &&
                Player.armor[2].type == ModContent.ItemType<BlackSmithLeggings>())
            {
                BlackSmithSet = true;

                // 採掘できる範囲 +3
                BlackSmithMiningRangeBonus = 3;

                Player.tileRangeX +=3;
                Player.tileRangeY +=3;
            }
        }
    }
}