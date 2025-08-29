using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevCrystalTaskForge.Content.Items
{
    public class TaskCrystal : ModItem
    {
        public string TaskId { get; set; } = "";
        public string TaskDescription { get; set; } = "";
        public string Priority { get; set; } = "Medium";
        public string Project { get; set; } = "";

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.maxStack = 1;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
            Item.value = Item.buyPrice(0, 0, 10, 0);
            
            // Set rarity based on priority
            Item.rare = GetRarityFromPriority(Priority);
        }

        private int GetRarityFromPriority(string priority)
        {
            return priority.ToLower() switch
            {
                "low" => ItemRarityID.White,
                "medium" => ItemRarityID.Blue,
                "high" => ItemRarityID.Orange,
                "urgent" => ItemRarityID.Red,
                "critical" => ItemRarityID.Purple,
                _ => ItemRarityID.White
            };
        }

        public override bool? UseItem(Player player)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                // Right-click to complete task
                CompleteTask(player);
            }
            return true;
        }

        private async void CompleteTask(Player player)
        {
            // Enhanced visual effects for task completion
            for (int i = 0; i < 30; i++)
            {
                Vector2 velocity = Main.rand.NextVector2CircularEdge(8f, 8f);
                Dust dust = Dust.NewDustPerfect(player.Center, DustID.CrystalSerpent, velocity);
                dust.noGravity = true;
                dust.scale = 1.2f + Main.rand.NextFloat() * 0.5f;
                dust.color = GetPriorityColor(Priority);
            }

            // Create shimmering completion effect
            for (int i = 0; i < 10; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Unit() * 3f;
                Dust sparkle = Dust.NewDustPerfect(player.Center + Main.rand.NextVector2Circular(20f, 20f), DustID.Enchanted_Gold, velocity);
                sparkle.noGravity = true;
                sparkle.fadeIn = 1.5f;
            }

            // Play completion sound with slight variation based on priority
            SoundStyle completionSound = Priority.ToLower() switch
            {
                "critical" => SoundID.Item29, // Epic sound for critical tasks
                "urgent" => SoundID.Item25,   // Urgent completion
                "high" => SoundID.Item4,      // Standard completion
                _ => SoundID.Item37           // Gentle completion for low/medium
            };
            SoundEngine.PlaySound(completionSound, player.position);

            // Send completion to MCP server
            try
            {
                var mcpClient = DevCrystalTaskForge.Instance.MCPClient;
                if (mcpClient != null && !string.IsNullOrEmpty(TaskId))
                {
                    string comment = $"Completed via Terraria - Player: {player.name}";
                    var response = await mcpClient.MarkTodoComplete(TaskId, comment);
                    
                    if (response.Success)
                    {
                        Main.NewText($"✨ Task completed: {TaskDescription}", Color.Gold);
                        Main.NewText($"Synchronized with MCP server", Color.LightGreen);
                        
                        // Bonus effect for successful sync
                        for (int i = 0; i < 5; i++)
                        {
                            Dust sync = Dust.NewDustPerfect(player.Center, DustID.DungeonSpirit, Main.rand.NextVector2Unit() * 2f);
                            sync.noGravity = true;
                        }
                    }
                    else
                    {
                        Main.NewText($"Task completed locally: {TaskDescription}", Color.Yellow);
                        Main.NewText($"MCP sync failed: {response.ErrorMessage}", Color.Orange);
                    }
                }
                else
                {
                    Main.NewText($"Task completed: {TaskDescription}", Color.LightGreen);
                    Main.NewText("MCP server not available", Color.Gray);
                }
            }
            catch (System.Exception ex)
            {
                DevCrystalTaskForge.Instance.Logger.Error($"Task completion error: {ex.Message}");
                Main.NewText($"Task completed locally: {TaskDescription}", Color.Yellow);
            }
            
            // Remove the crystal from inventory with a slight delay for effect
            Task.Delay(500).ContinueWith(_ => Item.TurnToAir());
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "TaskDescription", TaskDescription)
            {
                OverrideColor = Color.LightBlue
            });
            
            tooltips.Add(new TooltipLine(Mod, "Project", $"Project: {Project}")
            {
                OverrideColor = Color.Yellow
            });
            
            tooltips.Add(new TooltipLine(Mod, "Priority", $"Priority: {Priority}")
            {
                OverrideColor = GetPriorityColor(Priority)
            });
            
            tooltips.Add(new TooltipLine(Mod, "Usage", "Right-click to complete task")
            {
                OverrideColor = Color.Gray
            });
        }

        private Color GetPriorityColor(string priority)
        {
            return priority.ToLower() switch
            {
                "low" => Color.LightGray,
                "medium" => Color.LightBlue,
                "high" => Color.Orange,
                "urgent" => Color.Red,
                "critical" => Color.Purple,
                _ => Color.White
            };
        }
    }
}