using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using DevCrystalTaskForge.Content.Items;

namespace DevCrystalTaskForge.Common.Commands
{
    public class CreateTaskCommand : ModCommand
    {
        public override CommandType Type => CommandType.Chat;
        public override string Command => "createtask";
        public override string Usage => "/createtask [description] - Creates a new task crystal";
        public override string Description => "Creates a new task and gives you a task crystal";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            if (args.Length == 0)
            {
                caller.Reply("Usage: /createtask [priority] [project] [description]", Color.Red);
                caller.Reply("Example: /createtask high terraria Fix crystal completion effects", Color.Gray);
                caller.Reply("Priorities: low, medium, high, urgent, critical", Color.Gray);
                return;
            }

            // Parse arguments - priority and project are optional
            string priority = "medium";
            string project = "madness_interactive";
            string description = string.Join(" ", args);

            // Check if first arg is a priority
            if (args.Length > 1)
            {
                string firstArg = args[0].ToLower();
                if (new[] { "low", "medium", "high", "urgent", "critical" }.Contains(firstArg))
                {
                    priority = firstArg;
                    
                    // Check if second arg is a project
                    if (args.Length > 2)
                    {
                        string[] validProjects = { "madness_interactive", "omnispindle", "terraria", "devcrystal", "todomill", "inventorium" };
                        string secondArg = args[1].ToLower();
                        if (validProjects.Contains(secondArg))
                        {
                            project = secondArg;
                            description = string.Join(" ", args.Skip(2));
                        }
                        else
                        {
                            description = string.Join(" ", args.Skip(1));
                        }
                    }
                    else
                    {
                        description = string.Join(" ", args.Skip(1));
                    }
                }
            }

            Player player = caller.Player;
            _ = CreateTaskAsync(caller, player, description, project, priority);
        }

        private async System.Threading.Tasks.Task CreateTaskAsync(CommandCaller caller, Player player, string description, string project, string priority)
        {
            string taskId = System.Guid.NewGuid().ToString();
            
            try
            {
                // Try to create task via MCP server first
                var mcpClient = DevCrystalTaskForge.Instance.MCPClient;
                if (mcpClient != null)
                {
                    var response = await mcpClient.AddTodo(description, project, priority);
                    
                    if (response.Success)
                    {
                        // Extract task ID from response if available
                        if (response.Data?.todo_id != null)
                        {
                            taskId = response.Data.todo_id.ToString();
                        }
                        
                        caller.Reply($"✨ Task synchronized with MCP server", Color.Gold);
                    }
                    else
                    {
                        caller.Reply($"⚠️ MCP sync failed: {response.ErrorMessage}", Color.Orange);
                        caller.Reply("Creating local task crystal...", Color.Gray);
                    }
                }
            }
            catch (System.Exception ex)
            {
                DevCrystalTaskForge.Instance.Logger.Error($"MCP task creation failed: {ex.Message}");
                caller.Reply("⚠️ MCP server unavailable, creating local crystal", Color.Yellow);
            }

            // Create the task crystal regardless of MCP status
            int itemType = ModContent.ItemType<TaskCrystal>();
            Item newItem = new Item();
            newItem.SetDefaults(itemType);
            
            var taskCrystal = newItem.ModItem as TaskCrystal;
            if (taskCrystal != null)
            {
                taskCrystal.TaskDescription = description;
                taskCrystal.Priority = priority;
                taskCrystal.Project = project;
                taskCrystal.TaskId = taskId;
            }

            // Give the item to the player
            if (player.inventory[player.selectedItem].IsAir)
            {
                player.inventory[player.selectedItem] = newItem;
            }
            else
            {
                player.QuickSpawnItem(player.GetSource_FromThis(), newItem);
            }

            // Enhanced visual effect based on priority
            int particleCount = priority.ToLower() switch
            {
                "critical" => 25,
                "urgent" => 20,
                "high" => 15,
                _ => 10
            };

            Color effectColor = priority.ToLower() switch
            {
                "low" => Color.LightGray,
                "medium" => Color.LightBlue,
                "high" => Color.Orange,
                "urgent" => Color.Red,
                "critical" => Color.Purple,
                _ => Color.White
            };

            for (int i = 0; i < particleCount; i++)
            {
                Vector2 velocity = Main.rand.NextVector2CircularEdge(4f, 4f);
                Dust dust = Dust.NewDustPerfect(player.Center, DustID.CrystalSerpent, velocity);
                dust.noGravity = true;
                dust.color = effectColor;
                dust.scale = 1.0f + (particleCount / 25f);
            }

            // Play forging sound
            SoundEngine.PlaySound(SoundID.Item37, player.position);

            caller.Reply($"🔮 Task crystal forged: {description}", Color.LightGreen);
            caller.Reply($"Priority: {priority} | Project: {project}", Color.Cyan);
        }
    }

    public class ListTasksCommand : ModCommand
    {
        public override CommandType Type => CommandType.Chat;
        public override string Command => "listtasks";
        public override string Usage => "/listtasks - Lists all task crystals in your inventory";
        public override string Description => "Shows all task crystals you currently have";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            Player player = caller.Player;
            int taskCount = 0;

            caller.Reply("Current Task Crystals:", Color.Cyan);

            for (int i = 0; i < player.inventory.Length; i++)
            {
                Item item = player.inventory[i];
                if (item.ModItem is TaskCrystal taskCrystal)
                {
                    taskCount++;
                    Color priorityColor = taskCrystal.Priority.ToLower() switch
                    {
                        "low" => Color.LightGray,
                        "medium" => Color.LightBlue,
                        "high" => Color.Orange,
                        "urgent" => Color.Red,
                        "critical" => Color.Purple,
                        _ => Color.White
                    };

                    caller.Reply($"  [{taskCrystal.Priority}] {taskCrystal.TaskDescription}", priorityColor);
                }
            }

            if (taskCount == 0)
            {
                caller.Reply("No task crystals found. Use /createtask to forge new ones!", Color.Yellow);
            }
            else
            {
                caller.Reply($"Total: {taskCount} task crystals", Color.LightGreen);
            }
        }
    }

    public class SearchCrystalsCommand : ModCommand
    {
        public override CommandType Type => CommandType.Chat;
        public override string Command => "searchcrystals";
        public override string Usage => "/searchcrystals [query] - Search through your task crystals";
        public override string Description => "Search task crystals by description, project, or priority";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            if (args.Length == 0)
            {
                caller.Reply("Usage: /searchcrystals [search query]", Color.Red);
                caller.Reply("Example: /searchcrystals terraria", Color.Gray);
                caller.Reply("         /searchcrystals high priority", Color.Gray);
                return;
            }

            string query = string.Join(" ", args).ToLower();
            Player player = caller.Player;

            _ = SearchCrystalsAsync(caller, player, query);
        }

        private async System.Threading.Tasks.Task SearchCrystalsAsync(CommandCaller caller, Player player, string query)
        {
            caller.Reply($"🔍 Searching for: {query}", Color.Cyan);

            // Search local crystals first
            List<TaskCrystal> matchingCrystals = new List<TaskCrystal>();
            
            for (int i = 0; i < player.inventory.Length; i++)
            {
                Item item = player.inventory[i];
                if (item.ModItem is TaskCrystal taskCrystal)
                {
                    bool matches = taskCrystal.TaskDescription.ToLower().Contains(query) ||
                                 taskCrystal.Project.ToLower().Contains(query) ||
                                 taskCrystal.Priority.ToLower().Contains(query);
                    
                    if (matches)
                    {
                        matchingCrystals.Add(taskCrystal);
                    }
                }
            }

            // Display local results
            if (matchingCrystals.Count > 0)
            {
                caller.Reply($"📦 Local Crystals ({matchingCrystals.Count} matches):", Color.LightGreen);
                
                foreach (var crystal in matchingCrystals.Take(5)) // Limit to 5 for readability
                {
                    Color priorityColor = GetPriorityColor(crystal.Priority);
                    caller.Reply($"  🔮 [{crystal.Priority}] {crystal.TaskDescription}", priorityColor);
                    caller.Reply($"     Project: {crystal.Project}", Color.Gray);
                }
                
                if (matchingCrystals.Count > 5)
                {
                    caller.Reply($"     ... and {matchingCrystals.Count - 5} more matches", Color.Gray);
                }

                // Create highlighting effects for matching crystals
                foreach (var crystal in matchingCrystals)
                {
                    CreateCrystalHighlight(player, GetPriorityColor(crystal.Priority));
                }
            }
            else
            {
                caller.Reply("📦 No matching crystals found in inventory", Color.Yellow);
            }

            // Search MCP server if available
            try
            {
                var mcpClient = DevCrystalTaskForge.Instance.MCPClient;
                if (mcpClient != null)
                {
                    var response = await mcpClient.SearchTodos(query, 10);
                    
                    if (response.Success && response.Data?.items != null)
                    {
                        var serverResults = response.Data.items;
                        int serverCount = serverResults.Count;
                        
                        if (serverCount > 0)
                        {
                            caller.Reply($"☁️ Server Results ({serverCount} matches):", Color.Gold);
                            
                            int displayCount = 0;
                            foreach (var todo in serverResults)
                            {
                                if (displayCount >= 5) break; // Limit display
                                
                                string desc = todo.description?.ToString() ?? "No description";
                                string proj = todo.project?.ToString() ?? "Unknown";
                                string priority = todo.priority?.ToString() ?? "Medium";
                                string status = todo.status?.ToString() ?? "Unknown";
                                
                                Color statusColor = status.ToLower() switch
                                {
                                    "completed" => Color.Green,
                                    "pending" => Color.Yellow,
                                    _ => Color.White
                                };
                                
                                caller.Reply($"  ☁️ [{priority}] {desc}", GetPriorityColor(priority));
                                caller.Reply($"     Project: {proj} | Status: {status}", statusColor);
                                displayCount++;
                            }
                            
                            if (serverCount > 5)
                            {
                                caller.Reply($"     ... and {serverCount - 5} more on server", Color.Gray);
                            }
                        }
                        else
                        {
                            caller.Reply("☁️ No matching tasks found on server", Color.Gray);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                DevCrystalTaskForge.Instance.Logger.Error($"MCP search failed: {ex.Message}");
                caller.Reply("☁️ Server search unavailable", Color.Gray);
            }

            // Play search completion sound
            SoundEngine.PlaySound(SoundID.Item37, player.position);
        }

        private void CreateCrystalHighlight(Player player, Color color)
        {
            Vector2 highlightPos = player.Center + new Vector2(Main.rand.NextFloat(-40f, 40f), Main.rand.NextFloat(-25f, 25f));
            
            for (int i = 0; i < 5; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Unit() * 2f;
                Dust highlight = Dust.NewDustPerfect(highlightPos, DustID.BlueFairy, velocity);
                highlight.noGravity = true;
                highlight.color = color;
                highlight.alpha = 100;
                highlight.fadeIn = 1.2f;
            }
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