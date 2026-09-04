using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using MinerManagementMOD.Systems;

namespace MinerManagementMOD.NPCs
{
    public class JewelDragonNPC : ModNPC
    {
        // ============================================================
        // このドラゴンを所有しているプレイヤー
        // ============================================================

        public int OwnerPlayer = -1;


        // ============================================================
        // 追従設定
        // ============================================================

        private const float FollowDistance = 48f;
        private const float FollowHeight = 32f;

        private const float FollowSpeed = 0.12f;

        private const float FloatHeight = 5f;
        private const float FloatSpeed = 0.05f;


        // ============================================================
        // Static Defaults
        // ============================================================

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 1;

            NPCID.Sets.NoTownNPCHappiness[Type] = true;
        }


        // ============================================================
        // NPC設定
        // ============================================================

        public override void SetDefaults()
        {
            NPC.width = 64;
            NPC.height = 64;

            NPC.damage = 0;
            NPC.defense = 9999;
            NPC.lifeMax = 1;

            NPC.noGravity = true;
            NPC.noTileCollide = true;

            NPC.dontTakeDamage = true;

            NPC.friendly = true;

            NPC.aiStyle = -1;

            NPC.knockBackResist = 0f;

            NPC.value = 0f;

            NPC.timeLeft = 60;
        }


        // ============================================================
        // AI
        // ============================================================

        public override void AI()
        {
            NPC.timeLeft = 60;

            // 所有プレイヤーを確認
            if (OwnerPlayer < 0 ||
                OwnerPlayer >= Main.maxPlayers)
            {
                NPC.active = false;
                return;
            }

            Player player = Main.player[OwnerPlayer];

            if (!player.active || player.dead)
            {
                NPC.active = false;
                return;
            }


            // ========================================================
            // プレイヤーの後ろ
            // ========================================================

            Vector2 targetPosition =
                player.Center
                - new Vector2(
                    player.direction * FollowDistance,
                    FollowHeight
                );


            // ========================================================
            // ふわふわ
            // ========================================================

            float floatingOffset =
                (float)System.Math.Sin(
                    Main.GameUpdateCount * FloatSpeed
                ) * FloatHeight;

            targetPosition.Y += floatingOffset;


            // ========================================================
            // 追従
            // ========================================================

            Vector2 difference =
                targetPosition - NPC.Center;

            NPC.velocity =
                difference * FollowSpeed;

            if (difference.Length() < 2f)
            {
                NPC.velocity *= 0.5f;
            }


            // ========================================================
            // 向き
            // ========================================================

            NPC.direction = player.direction;
            NPC.spriteDirection = player.direction;
        }


        // ============================================================
        // 会話
        // ============================================================

        public override bool CanChat()
        {
            return true;
        }

        public override string GetChat()
        {
            Player player = Main.LocalPlayer;

            RainbowBuffPlayer modPlayer =
                player.GetModPlayer<RainbowBuffPlayer>();

            int level =
                modPlayer.RainbowLevel;

            string text =
                $"【レインボーバフ Lv.{level}】\n" +
                $"与えるダメージ　+{GetDamagePercent(level)}%\n" +
                $"受けるダメージ　-{GetDamageReductionPercent(level)}%\n" +
                $"採掘速度　　　　+{GetMiningPercent(level)}%\n" +
                $"移動速度　　　　+{GetMovePercent(level)}%\n" +
                $"攻撃速度　　　　+{GetAttackPercent(level)}%\n" +
                $"消費マナ　　　　-{GetManaPercent(level)}%\n\n";

            if (level >= 7)
            {
                text +=
                    "あなたはすべての宝石の力を捧げました。\n" +
                    "私の守護の力は、これ以上ないほど高まっています。";
            }
            else
            {
                int required =
                    RainbowBuffPlayer.GetRequiredForNextLevel(level);

                int progress =
                    modPlayer.GetCurrentLevelProgress();

                text +=
                    $"レインボージュエル：{progress} / {required}\n" +
                    "宝石の力を私に捧げてください。";
            }

            return text;
        }


        // ============================================================
        // 会話ボタン
        // ============================================================

        public override void SetChatButtons(
            ref string button,
            ref string button2)
        {
            RainbowBuffPlayer modPlayer =
                Main.LocalPlayer.GetModPlayer<RainbowBuffPlayer>();

            if (modPlayer.RainbowLevel >= 7)
            {
                button = "最大レベル";
            }
            else
            {
                button = "宝石をささげる";
            }

            button2 = "";
        }


        // ============================================================
        // ボタンクリック
        // ============================================================

        public override void OnChatButtonClicked(
            bool firstButton,
            ref string shopName)
        {
            if (!firstButton)
                return;

            RainbowBuffPlayer modPlayer =
                Main.LocalPlayer.GetModPlayer<RainbowBuffPlayer>();

            if (modPlayer.RainbowLevel >= 7)
            {
                Main.npcChatText =
                    "レインボーバフは最大レベルです。";
                return;
            }

            if (!modPlayer.OfferRainbowJewel())
            {
                Main.npcChatText =
                    "レインボージュエルを持っていません。";
                return;
            }

            // 奉納後、現在の状態を再表示
            Main.npcChatText =
                GetChat();
        }


        // ============================================================
        // 表示用数値
        // ============================================================

        private int GetDamagePercent(int level)
        {
            int[] values =
            {
                0, 10, 20, 30, 40, 50, 75, 100
            };

            return values[level];
        }


        private int GetDamageReductionPercent(int level)
        {
            int[] values =
            {
                0, 5, 10, 15, 20, 30, 40, 50
            };

            return values[level];
        }


        private int GetMiningPercent(int level)
        {
            int[] values =
            {
                0, 3, 6, 9, 12, 15, 20, 25
            };

            return values[level];
        }


        private int GetMovePercent(int level)
        {
            return GetMiningPercent(level);
        }


        private int GetAttackPercent(int level)
        {
            return GetMiningPercent(level);
        }


        private int GetManaPercent(int level)
        {
            int[] values =
            {
                0, 5, 10, 15, 20, 30, 40, 50
            };

            return values[level];
        }

        public override Color? GetAlpha(Color drawColor)
        {
            return Color.White;
        }

    }
}