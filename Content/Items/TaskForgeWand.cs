using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

namespace DevCrystalTaskForge.Content.Items
{
    public class TaskForgeWand : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 0;
            Item.DamageType = DamageClass.Magic;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 0;
            Item.value = Item.buyPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item8;
            Item.autoReuse = false;
            Item.noUseGraphic = false;
        }

        public override bool? UseItem(Player player)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                OpenTaskCreationUI(player);
            }
            return true;
        }

        private void OpenTaskCreationUI(Player player)
        {
            // Visual effect when casting the spell
            for (int i = 0; i < 15; i++)
            {
                Vector2 velocity = Main.rand.NextVector2CircularEdge(6f, 6f);
                Dust dust = Dust.NewDustPerfect(player.Center + new Vector2(0, -20), DustID.PurificationPowder, velocity);
                dust.noGravity = true;
                dust.scale = 1.0f;
            }

            // TODO: Open actual task creation UI
            Main.NewText("Task Creation Spell Cast! (UI coming soon)", Color.Cyan);
            Main.NewText("Use chat command: /createtask [description]", Color.LightBlue);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.ManaCrystal, 1)
                .AddIngredient(ItemID.FallenStar, 3)
                .AddIngredient(ItemID.Diamond, 1)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}