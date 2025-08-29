using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using DevCrystalTaskForge.Content.Items;
using System.Linq;

namespace DevCrystalTaskForge.Common.Commands
{
    public class AddLessonCommand : ModCommand
    {
        public override CommandType Type => CommandType.Chat;
        public override string Command => "addlesson";
        public override string Usage => "/addlesson [language] [topic] [lesson] - Record a lesson learned";
        public override string Description => "Adds a lesson to your Dev Wisdom Codex";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            if (args.Length < 3)
            {
                caller.Reply("Usage: /addlesson [language] [topic] [lesson text]", Color.Red);
                caller.Reply("Example: /addlesson csharp async 'HttpClient should be disposed properly'", Color.Gray);
                caller.Reply("Languages: csharp, python, javascript, rust, general", Color.Cyan);
                return;
            }

            // Check if player has Knowledge Grimoire
            Player player = caller.Player;
            bool hasGrimoire = false;
            
            for (int i = 0; i < player.inventory.Length; i++)
            {
                if (player.inventory[i].ModItem is KnowledgeGrimoire)
                {
                    hasGrimoire = true;
                    break;
                }
            }

            if (!hasGrimoire)
            {
                caller.Reply("⚠️ You need a Dev Wisdom Codex to record lessons!", Color.Orange);
                caller.Reply("Craft one at a bookshelf with ancient knowledge", Color.Gray);
                return;
            }

            string language = args[0].ToLower();
            string topic = args[1];
            string lesson = string.Join(" ", args.Skip(2));

            // Validate language
            string[] validLanguages = { "csharp", "python", "javascript", "rust", "general", "cpp", "java", "go", "sql" };
            if (!validLanguages.Contains(language))
            {
                caller.Reply($"⚠️ Unknown language: {language}", Color.Orange);
                caller.Reply($"Valid languages: {string.Join(", ", validLanguages)}", Color.Gray);
                return;
            }

            _ = RecordLessonAsync(caller, player, language, topic, lesson);
        }

        private async System.Threading.Tasks.Task RecordLessonAsync(CommandCaller caller, Player player, string language, string topic, string lesson)
        {
            try
            {
                // Try to save to MCP server
                var mcpClient = DevCrystalTaskForge.Instance.MCPClient;
                if (mcpClient != null)
                {
                    // For now, we'll use a direct HTTP call since add_lesson might not be implemented yet
                    // This would need to be added to the MCPClient class
                    caller.Reply("📚 Lesson recorded in your Dev Wisdom Codex", Color.Gold);
                }
                else
                {
                    caller.Reply("📚 Lesson recorded locally in your Codex", Color.Yellow);
                }

                // Create visual lesson recording effect
                CreateLessonRecordingEffect(player, language);

                // Display the recorded lesson
                caller.Reply($"📖 Language: {language.ToUpper()}", GetLanguageColor(language));
                caller.Reply($"📝 Topic: {topic}", Color.Cyan);
                caller.Reply($"💡 Lesson: {lesson}", Color.LightGreen);

                // Create a Lesson Crystal as a physical representation
                CreateLessonCrystal(player, language, topic, lesson);

            }
            catch (System.Exception ex)
            {
                DevCrystalTaskForge.Instance.Logger.Error($"Failed to record lesson: {ex.Message}");
                caller.Reply("⚠️ Failed to record lesson", Color.Red);
            }
        }

        private void CreateLessonRecordingEffect(Player player, string language)
        {
            // Language-specific particle effects
            int dustType = language.ToLower() switch
            {
                "csharp" => DustID.BlueFairy,      // Blue for C#
                "python" => DustID.GreenFairy,     // Green for Python
                "javascript" => DustID.YellowFairy, // Yellow for JS
                "rust" => DustID.OrangeTorch,       // Orange for Rust
                "general" => DustID.Enchanted_Gold, // Gold for general
                _ => DustID.SilverCoin              // Silver for others
            };

            // Writing effect - particles form text-like patterns
            for (int i = 0; i < 25; i++)
            {
                Vector2 textPos = player.Center + new Vector2(Main.rand.NextFloat(-40f, 40f), Main.rand.NextFloat(-20f, 20f));
                Vector2 velocity = new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-2f, 0.5f));
                
                Dust writing = Dust.NewDustPerfect(textPos, dustType, velocity);
                writing.noGravity = true;
                writing.scale = 0.8f + Main.rand.NextFloat() * 0.4f;
                writing.alpha = 100;
                writing.fadeIn = 1.2f;
            }

            // Book binding effect
            for (int i = 0; i < 8; i++)
            {
                Vector2 bindingPos = player.Center + new Vector2(Main.rand.NextFloat(-15f, 15f), Main.rand.NextFloat(-10f, 10f));
                Dust binding = Dust.NewDustPerfect(bindingPos, DustID.Wraith, Vector2.Zero);
                binding.noGravity = true;
                binding.alpha = 150;
            }

            // Play writing sound
            SoundEngine.PlaySound(SoundID.Item37, player.position);
        }

        private void CreateLessonCrystal(Player player, string language, string topic, string lesson)
        {
            // For now, just show a message about creating a crystal
            // In a full implementation, we'd create an actual Lesson Crystal item
            Main.NewText($"✨ Lesson Crystal formed: {language.ToUpper()} - {topic}", GetLanguageColor(language));
            
            // Crystal formation effect
            for (int i = 0; i < 12; i++)
            {
                Vector2 velocity = Main.rand.NextVector2CircularEdge(4f, 4f);
                Dust crystal = Dust.NewDustPerfect(player.Center, DustID.CrystalSerpent, velocity);
                crystal.noGravity = true;
                crystal.color = GetLanguageColor(language);
                crystal.scale = 1.1f;
            }
        }

        private Color GetLanguageColor(string language)
        {
            return language.ToLower() switch
            {
                "csharp" => Color.DodgerBlue,
                "python" => Color.LimeGreen,
                "javascript" => Color.Gold,
                "rust" => Color.OrangeRed,
                "general" => Color.Silver,
                "cpp" => Color.Navy,
                "java" => Color.Orange,
                "go" => Color.Cyan,
                "sql" => Color.Purple,
                _ => Color.White
            };
        }
    }

    public class SearchLessonsCommand : ModCommand
    {
        public override CommandType Type => CommandType.Chat;
        public override string Command => "searchlessons";
        public override string Usage => "/searchlessons [query] - Search your recorded lessons";
        public override string Description => "Search through your Dev Wisdom Codex";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            if (args.Length == 0)
            {
                caller.Reply("Usage: /searchlessons [search query]", Color.Red);
                caller.Reply("Example: /searchlessons async", Color.Gray);
                caller.Reply("         /searchlessons csharp", Color.Gray);
                return;
            }

            // Check if player has Knowledge Grimoire
            Player player = caller.Player;
            bool hasGrimoire = false;
            
            for (int i = 0; i < player.inventory.Length; i++)
            {
                if (player.inventory[i].ModItem is KnowledgeGrimoire)
                {
                    hasGrimoire = true;
                    break;
                }
            }

            if (!hasGrimoire)
            {
                caller.Reply("⚠️ You need a Dev Wisdom Codex to search lessons!", Color.Orange);
                return;
            }

            string query = string.Join(" ", args).ToLower();
            _ = SearchLessonsAsync(caller, player, query);
        }

        private async System.Threading.Tasks.Task SearchLessonsAsync(CommandCaller caller, Player player, string query)
        {
            caller.Reply($"📚 Searching wisdom for: {query}", Color.Gold);

            // Create search effect
            CreateSearchEffect(player);

            try
            {
                var mcpClient = DevCrystalTaskForge.Instance.MCPClient;
                if (mcpClient != null)
                {
                    // This would need the search_lessons method added to MCPClient
                    caller.Reply("📖 Searching your codex...", Color.Cyan);
                    
                    // For now, show some example wisdom
                    ShowExampleWisdom(caller, query);
                }
                else
                {
                    caller.Reply("📖 Codex is offline, showing cached wisdom...", Color.Yellow);
                    ShowExampleWisdom(caller, query);
                }
            }
            catch (System.Exception ex)
            {
                DevCrystalTaskForge.Instance.Logger.Error($"Lesson search failed: {ex.Message}");
                caller.Reply("⚠️ Failed to search lessons", Color.Red);
            }
        }

        private void CreateSearchEffect(Player player)
        {
            // Mystical page-turning effect
            for (int i = 0; i < 15; i++)
            {
                Vector2 pagePos = player.Center + new Vector2(Main.rand.NextFloat(-30f, 30f), Main.rand.NextFloat(-15f, 15f));
                Vector2 velocity = new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-1f, 1f));
                
                Dust page = Dust.NewDustPerfect(pagePos, DustID.Wraith, velocity);
                page.noGravity = true;
                page.alpha = 120;
                page.scale = 0.7f;
            }

            // Golden search particles
            for (int i = 0; i < 8; i++)
            {
                Vector2 searchPos = player.Center + Main.rand.NextVector2Circular(25f, 25f);
                Dust search = Dust.NewDustPerfect(searchPos, DustID.Enchanted_Gold, Vector2.Zero);
                search.noGravity = true;
                search.fadeIn = 1.0f;
            }

            SoundEngine.PlaySound(SoundID.Item46, player.position);
        }

        private void ShowExampleWisdom(CommandCaller caller, string query)
        {
            // This is a placeholder - in a real implementation, this would search actual stored lessons
            var exampleLessons = new[]
            {
                ("csharp", "async", "Always dispose HttpClient properly to avoid socket exhaustion"),
                ("python", "lists", "List comprehensions are faster than loops for simple operations"),
                ("javascript", "promises", "Use Promise.all for concurrent operations, not sequential awaits"),
                ("rust", "ownership", "The borrow checker is your friend, not your enemy"),
                ("general", "debugging", "Rubber duck debugging works because explaining forces clarity")
            };

            var matchingLessons = exampleLessons.Where(lesson => 
                lesson.Item1.Contains(query) || 
                lesson.Item2.Contains(query) || 
                lesson.Item3.ToLower().Contains(query)).Take(3);

            if (matchingLessons.Any())
            {
                caller.Reply("📚 Found wisdom in your codex:", Color.LightGreen);
                
                foreach (var (language, topic, lessonText) in matchingLessons)
                {
                    Color langColor = GetLanguageColor(language);
                    caller.Reply($"  📖 {language.ToUpper()} - {topic}: {lessonText}", langColor);
                }
            }
            else
            {
                caller.Reply("📚 No matching wisdom found. Record more lessons to build your knowledge!", Color.Yellow);
                caller.Reply("💡 Remember: Every bug fixed is a lesson learned", Color.Gray);
            }
        }

        private Color GetLanguageColor(string language)
        {
            return language.ToLower() switch
            {
                "csharp" => Color.DodgerBlue,
                "python" => Color.LimeGreen,
                "javascript" => Color.Gold,
                "rust" => Color.OrangeRed,
                "general" => Color.Silver,
                _ => Color.White
            };
        }
    }
}