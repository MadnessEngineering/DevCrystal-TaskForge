using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace DevCrystalTaskForge.Content.Items
{
    public class KnowledgeGrimoire : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.maxStack = 1;
            Item.useTurn = true;
            Item.autoReuse = false;
            Item.useAnimation = 45;
            Item.useTime = 45;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
            Item.value = Item.buyPrice(0, 2, 50, 0); // 2 gold 50 silver
            Item.rare = ItemRarityID.Orange;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dev Wisdom Codex");
            Tooltip.SetDefault("Records lessons learned from the depths of development\nRight-click to inscribe new wisdom\nCreates Lesson Crystals containing your hard-earned knowledge");
        }

        public override bool? UseItem(Player player)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                OpenLessonInterface(player);
            }
            return true;
        }

        private void OpenLessonInterface(Player player)
        {
            // Create a lesson entry prompt
            Main.NewText("📚 Dev Wisdom Codex activated!", Color.Gold);
            Main.NewText("Use /addlesson [language] [topic] [lesson] to record wisdom", Color.LightBlue);
            Main.NewText("Example: /addlesson csharp async 'Always dispose HttpClient properly'", Color.Gray);
            Main.NewText("Languages: csharp, python, javascript, rust, general", Color.Cyan);

            // Mystical tome opening effect
            for (int i = 0; i < 20; i++)
            {
                Vector2 velocity = Main.rand.NextVector2CircularEdge(5f, 5f);
                Dust dust = Dust.NewDustPerfect(player.Center, DustID.Enchanted_Gold, velocity);
                dust.noGravity = true;
                dust.scale = 1.4f;
                dust.alpha = 50;
            }

            // Ancient pages fluttering effect
            for (int i = 0; i < 8; i++)
            {
                Vector2 pageVel = new Vector2(Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-4f, -1f));
                Dust page = Dust.NewDustPerfect(player.Center + new Vector2(Main.rand.NextFloat(-20f, 20f), 10f), DustID.Wraith, pageVel);
                page.noGravity = true;
                page.alpha = 120;
                page.scale = 0.8f;
            }

            // Play ancient tome sound
            SoundEngine.PlaySound(SoundID.Item46, player.position);

            // Show some example lessons for inspiration
            ShowExampleLessons();
        }

        private void ShowExampleLessons()
        {
            string[] exampleLessons = {
                "💡 Tip: Document your mistakes - they become treasured knowledge!",
                "📝 Remember: Good variable names are self-documenting spells",
                "⚡ Wisdom: Test edge cases before they test you",
                "🔮 Truth: The best code is code that explains itself",
                "🛡️ Law: Always validate user input - users are chaotic neutral"
            };

            string randomLesson = Main.rand.Next(exampleLessons);
            Main.NewText(randomLesson, Color.LightGreen);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "WisdomCounter", "Contains the wisdom of countless debugging sessions")
            {
                OverrideColor = Color.Gold
            });
            
            tooltips.Add(new TooltipLine(Mod, "SearchTip", "Use /searchlessons to find recorded wisdom")
            {
                OverrideColor = Color.Cyan
            });
            
            tooltips.Add(new TooltipLine(Mod, "PowerSource", "Powered by developer tears and coffee")
            {
                OverrideColor = Color.Brown
            });
        }

        // Override the glow effect to make it look more mystical
        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            // Add a subtle pulsing glow effect
            float pulseScale = 1f + (float)Math.Sin(Main.GlobalTimeWrappedHourly * 3f) * 0.1f;
            
            // Draw glow effect
            for (int i = 0; i < 4; i++)
            {
                Vector2 glowOffset = new Vector2(2f, 0f).RotatedBy(MathHelper.PiOver2 * i) * pulseScale;
                spriteBatch.Draw(ModContent.Request<Texture2D>("DevCrystalTaskForge/Content/Items/KnowledgeGrimoire").Value,
                    position + glowOffset, frame, Color.Gold * 0.3f, 0f, origin, scale, SpriteEffects.None, 0f);
            }
        }
    }
}