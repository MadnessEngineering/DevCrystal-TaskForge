using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

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

        private void CompleteTask(Player player)
        {
            // Visual effects for task completion
            for (int i = 0; i < 20; i++)
            {
                Vector2 velocity = Main.rand.NextVector2CircularEdge(8f, 8f);
                Dust dust = Dust.NewDustPerfect(player.Center, DustID.CrystalSerpent, velocity);
                dust.noGravity = true;
                dust.scale = 1.2f;
            }

            // Play completion sound
            SoundEngine.PlaySound(SoundID.Item4, player.position);

            // TODO: Send completion to MCP server
            Main.NewText($"Task completed: {TaskDescription}", Color.LightGreen);
            
            // Remove the crystal from inventory
            Item.TurnToAir();
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