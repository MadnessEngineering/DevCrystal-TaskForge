using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using DevCrystalTaskForge.Common.Config;

namespace DevCrystalTaskForge
{
    public class DevCrystalTaskForge : Mod
    {
        internal static DevCrystalTaskForge Instance;
        internal MCPClient MCPClient { get; private set; }

        public override void Load()
        {
            Instance = this;
            
            // Initialize MCP client for task management
            MCPClient = new MCPClient();
            
            Logger.Info("DevCrystal TaskForge loaded - Ready to forge tasks with crystal magic!");
        }

        public override void Unload()
        {
            Instance = null;
            MCPClient?.Dispose();
            MCPClient = null;
        }

        public override void PostSetupContent()
        {
            // Initialize MCP connections after all content is loaded
            MCPClient?.Initialize();
        }
    }

    // Basic MCP client for communicating with task management servers
    public class MCPClient : System.IDisposable
    {
        private HttpClient httpClient;
        private string baseUrl = "http://localhost:8000"; // Default MCP server URL

        public MCPClient()
        {
            httpClient = new HttpClient();
        }

        public void Initialize()
        {
            var config = ModContent.GetInstance<TaskForgeConfig>();
            if (config.EnableMCP)
            {
                baseUrl = config.GetMCPBaseUrl();
                DevCrystalTaskForge.Instance.Logger.Info($"MCP Client initialized - connecting to {baseUrl}");
                
                // Test connection asynchronously
                _ = TestConnectionAsync();
            }
            else
            {
                DevCrystalTaskForge.Instance.Logger.Info("MCP integration disabled in config");
            }
        }

        private async void TestConnectionAsync()
        {
            bool connected = await TestConnection();
            if (connected)
            {
                DevCrystalTaskForge.Instance.Logger.Info("MCP server connection successful");
            }
            else
            {
                DevCrystalTaskForge.Instance.Logger.Warn("Could not connect to MCP server");
            }
        }

        public async Task<bool> TestConnection()
        {
            try
            {
                var response = await httpClient.GetAsync($"{baseUrl}/health");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public void Dispose()
        {
            httpClient?.Dispose();
        }
    }
}