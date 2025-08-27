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
                caller.Reply("Usage: /createtask [task description]", Color.Red);
                return;
            }

            string description = string.Join(" ", args);
            Player player = caller.Player;

            // Create a new task crystal
            int itemType = ModContent.ItemType<TaskCrystal>();
            Item newItem = new Item();
            newItem.SetDefaults(itemType);
            
            // Set task properties (for now, we'll use defaults)
            var taskCrystal = newItem.ModItem as TaskCrystal;
            if (taskCrystal != null)
            {
                taskCrystal.TaskDescription = description;
                taskCrystal.Priority = "Medium";
                taskCrystal.Project = "Default";
                taskCrystal.TaskId = System.Guid.NewGuid().ToString();
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

            // Visual effect
            for (int i = 0; i < 10; i++)
            {
                Vector2 velocity = Main.rand.NextVector2CircularEdge(4f, 4f);
                Dust dust = Dust.NewDustPerfect(player.Center, DustID.CrystalSerpent, velocity);
                dust.noGravity = true;
            }

            caller.Reply($"Task crystal forged: {description}", Color.LightGreen);
            
            // TODO: Send to MCP server
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
}