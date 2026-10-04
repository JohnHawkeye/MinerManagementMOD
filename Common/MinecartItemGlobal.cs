using Terraria;
using Terraria.ModLoader;
using MinerManagementMOD.Common.Players;

namespace MinerManagementMOD.Common
{
    public class MinecartItemGlobal : GlobalItem
    {
        // ==========================================
        // トロッコ中の追加回収範囲
        // ==========================================

        // ピクセル単位
        // 600px = 37.5ブロック
        private const int MinecartGrabRangeBonus = 600;

        // ==========================================
        // アイテム回収範囲を拡大
        // ==========================================

        public override void GrabRange(
            Item item,
            Player player,
            ref int grabRange)
        {
            MinecartPlayer minecartPlayer =
                player.GetModPlayer<MinecartPlayer>();

            if (!minecartPlayer.MinecartBuffActive)
                return;

            // 通常の回収範囲に600px追加
            grabRange += MinecartGrabRangeBonus;
        }
    }
}