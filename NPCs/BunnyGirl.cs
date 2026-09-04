using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using MinerManagementMOD.Items;
using MinerManagementMOD.Systems;
using MinerManagementMOD.Projectiles;

namespace MinerManagementMOD.NPCs
{
    [AutoloadHead]
    public class BunnyGirl : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 23;

            NPCID.Sets.ExtraFramesCount[Type] = 0;
            NPCID.Sets.AttackFrameCount[Type] = 1;
            NPCID.Sets.DangerDetectRange[Type] = 500;
            NPCID.Sets.AttackType[Type] = 1;
            NPCID.Sets.AttackTime[Type] = 30;
            NPCID.Sets.AttackAverageChance[Type] = 10;
            NPCID.Sets.HatOffsetY[Type] = 4;

        }

        public override void SetDefaults()
        {
            NPC.townNPC = true;
            NPC.friendly = true;

            NPC.width = 20;
            NPC.height = 20;

            NPC.aiStyle = NPCAIStyleID.Passive;

            NPC.defense = 35;
            NPC.lifeMax = 300;

            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;

            NPC.knockBackResist = 0.5f;

            AnimationType = NPCID.Nurse;

        }
        public override void AI()
        {
            foreach (Player player in Main.ActivePlayers)
            {
                if (!player.active || player.dead)
                    continue;

                // プレイヤーとの距離
                float xDist = Math.Abs(player.Center.X - NPC.Center.X);
                float yDist = Math.Abs(player.Center.Y - NPC.Center.Y);

                if (xDist < 48f && yDist < 24f)
                {
                    // ラブラブなハートを出す
                    if (Main.rand.NextBool(40))
                    {
                        Vector2 pos = NPC.Center +
                            new Vector2(
                                Main.rand.NextFloat(-8f, 8f),
                                -24f);

                        Projectile.NewProjectile(
                            NPC.GetSource_FromAI(),
                            pos,
                            new Vector2(
                                Main.rand.NextFloat(-1f, 1f),
                                -0.5f),
                            ModContent.ProjectileType<LoveHeart>(),
                            0,
                            0f,
                            Main.myPlayer);
                    }

                }
            }
        }
        public override bool CanTownNPCSpawn(int numTownNPCs)
        {
            int minerCoins = ModContent.ItemType<CopperMinerCoin>();

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (player != null && player.active && player.HasItem(minerCoins))
                {
                    return true;
                }
            }
            return false;
        }

        public override List<string> SetNPCNameList()
        {
            return new List<string>()
            {
                "バニーガール"
            };
        }

        public override string GetChat()
        {
            return "お客さん、遊んでいってね。";
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = "交換ショップ";
            button2 = "ショップについて";
        }

        public override void OnChatButtonClicked(bool firstButton, ref string shop)
        {
            if (firstButton)
            {
                shop = "Shop";
            }
            else
            {

                Main.npcChatText =
                    "マイナーコインで便利なアイテムと交換できるよん。";
            }
        }

        public override void AddShops()
        {
            var npcShop = new NPCShop(Type);

            npcShop.Add(new Item(ModContent.ItemType<MinerBlessingPotion>())
            {
                shopCustomPrice = 10,
                shopSpecialCurrency = MinerCoinCurrencySystem.GoldCurrencyID
            });

            npcShop.Add(new Item(ItemID.FishFinder)
            {
                shopCustomPrice = 1,
                shopSpecialCurrency = MinerCoinCurrencySystem.PlatinumCurrencyID
            });

            npcShop.Add(new Item(ModContent.ItemType<SlopeAccessory>())
            {
                shopCustomPrice = 20,
                shopSpecialCurrency = MinerCoinCurrencySystem.GoldCurrencyID
            });

            npcShop.Add(new Item(ModContent.ItemType<OreSeed>())
            {
                shopCustomPrice = 90,
                shopSpecialCurrency = MinerCoinCurrencySystem.SilverCurrencyID
            });

            npcShop.Add(new Item(ModContent.ItemType<TradeExchangeMachine>())
            {
                shopCustomPrice = 1,
                shopSpecialCurrency = MinerCoinCurrencySystem.PlatinumCurrencyID
            });

            npcShop.Add(new Item(ModContent.ItemType<StoneWand>())
            {
                shopCustomPrice = 1,
                shopSpecialCurrency = MinerCoinCurrencySystem.PlatinumCurrencyID
            });

            npcShop.Add(new Item(ModContent.ItemType<SkyIslandMap>())
            {
                shopCustomPrice = 10,
                shopSpecialCurrency = MinerCoinCurrencySystem.GoldCurrencyID
            });

            npcShop.Add(new Item(ModContent.ItemType<DungeonMap>())
            {
                shopCustomPrice = 10,
                shopSpecialCurrency = MinerCoinCurrencySystem.GoldCurrencyID
            });

            npcShop.Add(new Item(ItemID.SoulofLight)
            {
                shopCustomPrice = 1,
                shopSpecialCurrency = MinerCoinCurrencySystem.GoldCurrencyID
            },Condition.Hardmode);

            npcShop.Add(new Item(ItemID.SoulofNight)
            {
                shopCustomPrice = 1,
                shopSpecialCurrency = MinerCoinCurrencySystem.GoldCurrencyID
            },Condition.Hardmode);

            npcShop.Register();
        }

        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        {
            damage = 15;
            knockback = 2f;
        }

        public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
        {
            cooldown = 5;
            randExtraCooldown = 10;
        }

        public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
        {
            projType = ModContent.ProjectileType<LoveHeart>();
            attackDelay = 1;
        }

        public override void TownNPCAttackProjSpeed(
            ref float multiplier,
            ref float gravityCorrection,
            ref float randomOffset)
        {
            multiplier = 7f;
        }

        public override void OnKill()
        {

            CombatText.NewText(
                new Rectangle(
                    (int)NPC.Center.X,
                    (int)NPC.Center.Y,
                    16,
                    16
                ),
                Color.Pink,
                "きゃぁ～ん"
            );
        }
    }
}