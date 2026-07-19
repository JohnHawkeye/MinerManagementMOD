using MinerManagementMOD.Common.UI;
using Terraria;
using Terraria.ModLoader;

namespace MinerManagementMOD.Common.Players
{
    public class UIPlayer : ModPlayer
    {
        public override void ResetEffects()
        {
            // 鉱夫名簿
            if (MinerUISystem.Visible)
            {
                DisableItemUse();
            }

            // 売却画面
            // if (MarketUISystem.Visible)
            // {
            //     DisableItemUse();
            // }

            // 今後追加予定
            // if (QuestUISystem.Visible)
            // {
            //     DisableItemUse();
            // }
        }

        /// <summary>
        /// アイテムの使用・設置・採掘を禁止
        /// </summary>
        private void DisableItemUse()
        {
            Player.controlUseItem = false;
            Player.controlUseTile = false;

            Player.releaseUseItem = false;
            Player.releaseUseTile = false;
        }
    }
}