using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MinerManagementMOD.Common.Players
{
    public class MinecartPlayer : ModPlayer
    {
        // ==========================================
        // 設定
        // ==========================================

        // 設置範囲の追加量
        private const int BuildRangeBonus = 6;

        // ==========================================
        // トロッコに乗っているか
        // ==========================================

        public bool MinecartBuffActive
        {
            get
            {
                return Player.mount.Active &&
                       Player.mount.Type == MountID.Minecart;
            }
        }

        // ==========================================
        // 毎フレームの初期化
        // ==========================================

        public override void ResetEffects()
        {
            // ここでは何もしません。
            //
            // blockRange は毎フレームの初期化後に
            // PostUpdateEquips() で追加します。
        }

        // ==========================================
        // トロッコ中にバフを付与
        // ==========================================

        public override void PreUpdateBuffs()
        {
            if (!MinecartBuffActive)
                return;

            int buffType =
                ModContent.BuffType<Buffs.MinecartProtectionBuff>();

            // 常に1秒分を維持
            Player.AddBuff(buffType, 60);
        }

        // ==========================================
        // 設置範囲 +6
        // ==========================================

        public override void PostUpdateEquips()
        {
            if (!MinecartBuffActive)
                return;

            Player.blockRange += BuildRangeBonus;
            // 採掘範囲 +6
            Player.tileRangeX += BuildRangeBonus;
            Player.tileRangeY += BuildRangeBonus;
        }

        // ==========================================
        // NPCからの攻撃を完全無効化
        // ==========================================

        public override bool CanBeHitByNPC(
            NPC npc,
            ref int cooldownSlot)
        {
            if (MinecartBuffActive)
            {
                return false;
            }

            return true;
        }

        // ==========================================
        // 敵Projectileからの攻撃を完全無効化
        // ==========================================

        public override bool CanBeHitByProjectile(
            Projectile proj)
        {
            if (MinecartBuffActive)
            {
                return false;
            }

            return true;
        }

        // ==========================================
        // 最終的なダメージ処理でも完全回避
        // ==========================================

        public override bool FreeDodge(Player.HurtInfo info)
        {
            if (MinecartBuffActive)
            {
                return true;
            }

            return false;
        }

        // ==========================================
        // デバフを無効化
        // ==========================================

        public override void PostUpdateBuffs()
        {
            if (!MinecartBuffActive)
                return;

            // 現在存在する全てのデバフを削除
            for (int i = 0; i < Player.MaxBuffs; i++)
            {
                if (Player.buffTime[i] <= 0)
                    continue;

                int buffType = Player.buffType[i];

                if (buffType < 0 || buffType >= Main.debuff.Length)
                    continue;

                if (Main.debuff[buffType])
                {
                    Player.DelBuff(i);
                    i--;
                }
            }

            // 今後付与されるデバフも免疫にする
            for (int i = 0; i < Player.buffImmune.Length; i++)
            {
                if (i < Main.debuff.Length &&
                    Main.debuff[i])
                {
                    Player.buffImmune[i] = true;
                }
            }
        }
    }
}