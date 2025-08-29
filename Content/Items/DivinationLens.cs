using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace DevCrystalTaskForge.Content.Items
{
    public class DivinationLens : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 28;
            Item.maxStack = 1;
            Item.useTurn = true;
            Item.autoReuse = false;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
            Item.value = Item.buyPrice(0, 1, 0, 0); // 1 gold
            Item.rare = ItemRarityID.Blue;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Divination Crystal Lens");
            Tooltip.SetDefault("Right-click to search your task crystals\nMakes matching crystals glow with mystical energy");
        }

        public override bool? UseItem(Player player)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                // Open search interface (for now, use chat interface)
                OpenSearchInterface(player);
            }
            return true;
        }

        private void OpenSearchInterface(Player player)
        {
            // Create a simple search prompt
            Main.NewText("🔮 Divination Lens activated! Type your search query:", Color.Cyan);
            Main.NewText("Example: 'high priority' or 'terraria' or 'fix'", Color.Gray);
            Main.NewText("Use /searchcrystals [query] to search", Color.LightBlue);

            // Visual effect
            for (int i = 0; i < 15; i++)
            {
                Vector2 velocity = Main.rand.NextVector2CircularEdge(6f, 6f);
                Dust dust = Dust.NewDustPerfect(player.Center, DustID.BlueFairy, velocity);
                dust.noGravity = true;
                dust.scale = 1.3f;
                dust.alpha = 100;
            }

            // Play mystical sound
            SoundEngine.PlaySound(SoundID.Item46, player.position);

            // Briefly highlight all task crystals in inventory
            HighlightTaskCrystals(player);
        }

        private void HighlightTaskCrystals(Player player)
        {
            int crystalCount = 0;
            
            for (int i = 0; i < player.inventory.Length; i++)
            {
                Item item = player.inventory[i];
                if (item.ModItem is TaskCrystal)
                {
                    crystalCount++;
                    
                    // Create gentle glow effect around task crystals
                    Vector2 itemPos = player.Center + new Vector2(Main.rand.NextFloat(-50f, 50f), Main.rand.NextFloat(-30f, 30f));
                    
                    for (int j = 0; j < 3; j++)
                    {
                        Vector2 dustVel = Main.rand.NextVector2Unit() * 1.5f;
                        Dust glow = Dust.NewDustPerfect(itemPos, DustID.BlueFairy, dustVel);
                        glow.noGravity = true;
                        glow.fadeIn = 1.0f;
                        glow.alpha = 150;
                    }
                }
            }

            if (crystalCount == 0)
            {
                Main.NewText("No task crystals found in inventory", Color.Yellow);
            }
            else
            {
                Main.NewText($"Found {crystalCount} task crystals", Color.LightGreen);
            }
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod, "SearchTip", "Reveals hidden properties of task crystals")
            {
                OverrideColor = Color.LightBlue
            });
            
            tooltips.Add(new TooltipLine(Mod, "MCPIntegration", "Searches both local and MCP server tasks")
            {
                OverrideColor = Color.Cyan
            });
        }
    }
}