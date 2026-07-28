using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace MinerManagementMOD.Items
{
    // Crystal Vacuum で採掘したアイテムだけを対象プレイヤーへ吸い寄せるためのGlobalItem
    public class CrystalVacuumGlobalItem : GlobalItem
    {
        public override bool InstancePerEntity => true;

        private bool isVacuuming = false;
        private int targetPlayerIndex = -1;

        public void StartVacuum(int playerIndex)
        {
            isVacuuming = true;
            targetPlayerIndex = playerIndex;
        }

        public override void PostUpdate(Item item)
        {
            if (!isVacuuming)
                return;

            if (targetPlayerIndex < 0 || targetPlayerIndex >= Main.maxPlayers)
            {
                isVacuuming = false;
                return;
            }

            Player target = Main.player[targetPlayerIndex];
            if (!target.active || target.dead)
            {
                isVacuuming = false;
                return;
            }

            Vector2 toPlayer = target.Center - item.Center;
            float distance = toPlayer.Length();

            // 十分近づいたらバニラの拾得処理に任せて終了
            if (distance < 24f)
            {
                isVacuuming = false;
                return;
            }

            toPlayer.Normalize();

            // プレイヤーに近づくほど加速し、吸い込まれるような演出にする
            float speed = MathHelper.Lerp(6f, 18f, MathHelper.Clamp(1f - distance / 500f, 0f, 1f));
            item.velocity = toPlayer * speed;
        }
    }
}