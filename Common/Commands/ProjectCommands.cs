using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System.Linq;

namespace DevCrystalTaskForge.Common.Commands
{
    public class ListProjectsCommand : ModCommand
    {
        public override CommandType Type => CommandType.Chat;
        public override string Command => "projects";
        public override string Usage => "/projects - List all available development projects";
        public override string Description => "Shows all projects you can work on";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            Player player = caller.Player;
            _ = ListProjectsAsync(caller, player);
        }

        private async System.Threading.Tasks.Task ListProjectsAsync(CommandCaller caller, Player player)
        {
            caller.Reply("🏛️ Scanning the realm of active projects...", Color.Cyan);

            // Create project scanning effect
            CreateProjectScanEffect(player);

            try
            {
                var mcpClient = DevCrystalTaskForge.Instance.MCPClient;
                if (mcpClient != null)
                {
                    var response = await mcpClient.ListProjects();
                    
                    if (response.Success && response.Data?.projects != null)
                    {
                        var projects = response.Data.projects;
                        caller.Reply($"📊 Found {projects.Count} active projects:", Color.Gold);

                        int displayCount = 0;
                        foreach (var project in projects)
                        {
                            if (displayCount >= 10) break; // Limit display
                            
                            string projectName = project.ToString();
                            Color projectColor = GetProjectColor(projectName);
                            string icon = GetProjectIcon(projectName);
                            
                            caller.Reply($"  {icon} {projectName.Replace("_", " ").ToTitleCase()}", projectColor);
                            displayCount++;
                        }

                        if (projects.Count > 10)
                        {
                            caller.Reply($"  ... and {projects.Count - 10} more projects", Color.Gray);
                        }

                        caller.Reply("💡 Use project names in /createtask [priority] [project] [task]", Color.LightBlue);
                    }
                    else
                    {
                        caller.Reply("⚠️ Could not retrieve projects from server", Color.Orange);
                        ShowDefaultProjects(caller);
                    }
                }
                else
                {
                    caller.Reply("📊 Server offline, showing known projects:", Color.Yellow);
                    ShowDefaultProjects(caller);
                }
            }
            catch (System.Exception ex)
            {
                DevCrystalTaskForge.Instance.Logger.Error($"Project listing failed: {ex.Message}");
                caller.Reply("⚠️ Project scan failed, showing defaults", Color.Red);
                ShowDefaultProjects(caller);
            }
        }

        private void CreateProjectScanEffect(Player player)
        {
            // Scanning beam effect
            for (int i = 0; i < 20; i++)
            {
                float angle = (float)i / 20f * MathHelper.TwoPi;
                Vector2 scanPos = player.Center + new Vector2(30f, 0f).RotatedBy(angle);
                Vector2 velocity = Vector2.UnitY.RotatedBy(angle) * 2f;
                
                Dust scan = Dust.NewDustPerfect(scanPos, DustID.BlueFairy, velocity);
                scan.noGravity = true;
                scan.scale = 0.8f;
                scan.alpha = 150;
            }

            // Central portal effect
            for (int i = 0; i < 8; i++)
            {
                Vector2 portalVel = Main.rand.NextVector2Unit() * 1.5f;
                Dust portal = Dust.NewDustPerfect(player.Center, DustID.DungeonSpirit, portalVel);
                portal.noGravity = true;
                portal.fadeIn = 1.2f;
            }

            SoundEngine.PlaySound(SoundID.Item15, player.position);
        }

        private void ShowDefaultProjects(CommandCaller caller)
        {
            var defaultProjects = new[]
            {
                ("madness_interactive", "🏗️"),
                ("omnispindle", "🌪️"),
                ("terraria", "⚔️"),
                ("inventorium", "📦"),
                ("todomill", "✅"),
                ("hammerspoon", "🔨"),
                ("devcrystal", "💎")
            };

            foreach (var (project, icon) in defaultProjects)
            {
                Color projectColor = GetProjectColor(project);
                caller.Reply($"  {icon} {project.Replace("_", " ").ToTitleCase()}", projectColor);
            }

            caller.Reply("💡 These are the core development realms", Color.Gray);
        }

        private Color GetProjectColor(string projectName)
        {
            return projectName.ToLower() switch
            {
                "madness_interactive" => Color.Purple,
                "omnispindle" => Color.Cyan,
                "terraria" => Color.Green,
                "inventorium" => Color.Orange,
                "todomill" => Color.LightBlue,
                "hammerspoon" => Color.Brown,
                "devcrystal" => Color.MediumPurple,
                "taskforge" => Color.Gold,
                _ => Color.White
            };
        }

        private string GetProjectIcon(string projectName)
        {
            return projectName.ToLower() switch
            {
                "madness_interactive" => "🏗️",
                "omnispindle" => "🌪️",
                "terraria" => "⚔️",
                "inventorium" => "📦",
                "todomill" => "✅",
                "hammerspoon" => "🔨",
                "devcrystal" => "💎",
                "taskforge" => "⚒️",
                _ => "📁"
            };
        }
    }

    public class ProjectTasksCommand : ModCommand
    {
        public override CommandType Type => CommandType.Chat;
        public override string Command => "projecttasks";
        public override string Usage => "/projecttasks [project] - Show tasks for a specific project";
        public override string Description => "Lists recent tasks for a given project";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            if (args.Length == 0)
            {
                caller.Reply("Usage: /projecttasks [project name]", Color.Red);
                caller.Reply("Example: /projecttasks terraria", Color.Gray);
                caller.Reply("Use /projects to see available projects", Color.Cyan);
                return;
            }

            string projectName = string.Join("_", args).ToLower();
            Player player = caller.Player;

            _ = ShowProjectTasksAsync(caller, player, projectName);
        }

        private async System.Threading.Tasks.Task ShowProjectTasksAsync(CommandCaller caller, Player player, string projectName)
        {
            caller.Reply($"📋 Fetching tasks for project: {projectName.Replace("_", " ").ToTitleCase()}", Color.Cyan);

            // Create project focus effect
            CreateProjectFocusEffect(player, projectName);

            try
            {
                // First check local crystals
                int localCount = 0;
                for (int i = 0; i < player.inventory.Length; i++)
                {
                    Item item = player.inventory[i];
                    if (item.ModItem is TaskCrystal crystal && crystal.Project.ToLower() == projectName)
                    {
                        if (localCount == 0)
                        {
                            caller.Reply("📦 Local Task Crystals:", Color.LightGreen);
                        }
                        
                        if (localCount < 5) // Limit display
                        {
                            Color priorityColor = GetPriorityColor(crystal.Priority);
                            caller.Reply($"  🔮 [{crystal.Priority}] {crystal.TaskDescription}", priorityColor);
                        }
                        localCount++;
                    }
                }

                if (localCount > 5)
                {
                    caller.Reply($"  ... and {localCount - 5} more local crystals", Color.Gray);
                }

                // Try to get server tasks
                var mcpClient = DevCrystalTaskForge.Instance.MCPClient;
                if (mcpClient != null)
                {
                    // This would use a list_project_todos endpoint
                    caller.Reply("☁️ Checking server for project tasks...", Color.Gold);
                    caller.Reply("🚧 Server task fetching coming soon!", Color.Yellow);
                }
                
                if (localCount == 0)
                {
                    caller.Reply($"📋 No tasks found for project: {projectName}", Color.Yellow);
                    caller.Reply($"Use /createtask medium {projectName} [description] to create one!", Color.LightBlue);
                }

            }
            catch (System.Exception ex)
            {
                DevCrystalTaskForge.Instance.Logger.Error($"Project task listing failed: {ex.Message}");
                caller.Reply("⚠️ Failed to fetch project tasks", Color.Red);
            }
        }

        private void CreateProjectFocusEffect(Player player, string projectName)
        {
            Color projectColor = GetProjectColor(projectName);
            
            // Project aura effect
            for (int i = 0; i < 15; i++)
            {
                Vector2 auraPos = player.Center + Main.rand.NextVector2Circular(40f, 40f);
                Vector2 velocity = (player.Center - auraPos).SafeNormalize(Vector2.UnitY) * 1.5f;
                
                Dust aura = Dust.NewDustPerfect(auraPos, DustID.BlueFairy, velocity);
                aura.noGravity = true;
                aura.color = projectColor;
                aura.scale = 1.2f;
                aura.alpha = 120;
            }

            SoundEngine.PlaySound(SoundID.Item37, player.position);
        }

        private Color GetProjectColor(string projectName)
        {
            return projectName.ToLower() switch
            {
                "madness_interactive" => Color.Purple,
                "omnispindle" => Color.Cyan,
                "terraria" => Color.Green,
                "inventorium" => Color.Orange,
                "todomill" => Color.LightBlue,
                "hammerspoon" => Color.Brown,
                "devcrystal" => Color.MediumPurple,
                _ => Color.White
            };
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

// Extension method for title case
public static class StringExtensions
{
    public static string ToTitleCase(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        var words = input.Split(' ');
        for (int i = 0; i < words.Length; i++)
        {
            if (words[i].Length > 0)
            {
                words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1).ToLower();
            }
        }
        return string.Join(" ", words);
    }
}