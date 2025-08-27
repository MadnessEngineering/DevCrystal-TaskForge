using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace DevCrystalTaskForge.Common.Config
{
    public class TaskForgeConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("MCP Server Settings")]
        
        [DefaultValue("localhost")]
        [Label("MCP Server Host")]
        [Tooltip("The hostname or IP address of your MCP server")]
        public string MCPHost { get; set; } = "localhost";

        [DefaultValue(8000)]
        [Label("MCP Server Port")]
        [Tooltip("The port number for your MCP server")]
        [Range(1, 65535)]
        public int MCPPort { get; set; } = 8000;

        [DefaultValue(true)]
        [Label("Enable MCP Integration")]
        [Tooltip("Enable or disable MCP server communication")]
        public bool EnableMCP { get; set; } = true;

        [Header("Gameplay Settings")]

        [DefaultValue(true)]
        [Label("Task Meteors")]
        [Tooltip("Enable task meteors for urgent new tasks")]
        public bool EnableTaskMeteors { get; set; } = true;

        [DefaultValue(30)]
        [Label("Meteor Frequency (seconds)")]
        [Tooltip("How often task meteors can appear")]
        [Range(10, 300)]
        public int MeteorFrequency { get; set; } = 30;

        [DefaultValue(true)]
        [Label("Completion Effects")]
        [Tooltip("Enable visual effects when completing tasks")]
        public bool EnableCompletionEffects { get; set; } = true;

        [DefaultValue(true)]
        [Label("Agent NPCs")]
        [Tooltip("Enable AI agent NPCs in the world")]
        public bool EnableAgentNPCs { get; set; } = true;

        [Header("UI Settings")]

        [DefaultValue(true)]
        [Label("Show Task Tooltips")]
        [Tooltip("Show detailed tooltips on task crystals")]
        public bool ShowTaskTooltips { get; set; } = true;

        [DefaultValue(true)]
        [Label("Priority Colors")]
        [Tooltip("Use colors to indicate task priority")]
        public bool UsePriorityColors { get; set; } = true;

        public string GetMCPBaseUrl()
        {
            return $"http://{MCPHost}:{MCPPort}";
        }
    }
}