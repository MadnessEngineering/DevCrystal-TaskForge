using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using System;
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

    // Enhanced MCP client for communicating with Omnispindle task management server
    public class MCPClient : System.IDisposable
    {
        private HttpClient httpClient;
        private string baseUrl = "http://localhost:8000"; // Default MCP server URL
        private bool isConnected = false;

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
            isConnected = await TestConnection();
            if (isConnected)
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

        // Enhanced MCP Tool Methods
        
        public async Task<MCPResponse> AddTodo(string description, string project, string priority = "Medium")
        {
            if (!isConnected) return MCPResponse.Failure("Not connected to MCP server");
            
            try
            {
                var payload = new
                {
                    description = description,
                    project = project,
                    priority = priority,
                    target_agent = "terraria_player"
                };
                
                var json = JsonConvert.SerializeObject(payload);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                
                var response = await httpClient.PostAsync($"{baseUrl}/api/todos", content);
                var responseText = await response.Content.ReadAsStringAsync();
                
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<dynamic>(responseText);
                    return MCPResponse.Success(result);
                }
                else
                {
                    return MCPResponse.Failure($"Server error: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                DevCrystalTaskForge.Instance.Logger.Error($"AddTodo failed: {ex.Message}");
                return MCPResponse.Failure(ex.Message);
            }
        }
        
        public async Task<MCPResponse> MarkTodoComplete(string todoId, string comment = null)
        {
            if (!isConnected) return MCPResponse.Failure("Not connected to MCP server");
            
            try
            {
                var payload = new
                {
                    todo_id = todoId,
                    comment = comment
                };
                
                var json = JsonConvert.SerializeObject(payload);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                
                var response = await httpClient.PutAsync($"{baseUrl}/api/todos/{todoId}/complete", content);
                var responseText = await response.Content.ReadAsStringAsync();
                
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<dynamic>(responseText);
                    return MCPResponse.Success(result);
                }
                else
                {
                    return MCPResponse.Failure($"Server error: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                DevCrystalTaskForge.Instance.Logger.Error($"MarkTodoComplete failed: {ex.Message}");
                return MCPResponse.Failure(ex.Message);
            }
        }
        
        public async Task<MCPResponse> SearchTodos(string query, int limit = 20)
        {
            if (!isConnected) return MCPResponse.Failure("Not connected to MCP server");
            
            try
            {
                var encodedQuery = System.Web.HttpUtility.UrlEncode(query);
                var response = await httpClient.GetAsync($"{baseUrl}/api/todos/search?query={encodedQuery}&limit={limit}");
                var responseText = await response.Content.ReadAsStringAsync();
                
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<dynamic>(responseText);
                    return MCPResponse.Success(result);
                }
                else
                {
                    return MCPResponse.Failure($"Server error: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                DevCrystalTaskForge.Instance.Logger.Error($"SearchTodos failed: {ex.Message}");
                return MCPResponse.Failure(ex.Message);
            }
        }
        
        public async Task<MCPResponse> ListProjects()
        {
            if (!isConnected) return MCPResponse.Failure("Not connected to MCP server");
            
            try
            {
                var response = await httpClient.GetAsync($"{baseUrl}/api/projects");
                var responseText = await response.Content.ReadAsStringAsync();
                
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<dynamic>(responseText);
                    return MCPResponse.Success(result);
                }
                else
                {
                    return MCPResponse.Failure($"Server error: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                DevCrystalTaskForge.Instance.Logger.Error($"ListProjects failed: {ex.Message}");
                return MCPResponse.Failure(ex.Message);
            }
        }

        public void Dispose()
        {
            httpClient?.Dispose();
        }
    }

    // Response wrapper for MCP operations
    public class MCPResponse
    {
        public bool Success { get; set; }
        public dynamic Data { get; set; }
        public string ErrorMessage { get; set; }

        public static MCPResponse Success(dynamic data)
        {
            return new MCPResponse { Success = true, Data = data };
        }

        public static MCPResponse Failure(string error)
        {
            return new MCPResponse { Success = false, ErrorMessage = error };
        }
    }
}