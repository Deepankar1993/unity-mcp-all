using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;

namespace UnityMcpBridge.Editor.Helpers
{
    /// <summary>
    /// Helper class for generating WSL-specific MCP configurations
    /// </summary>
    public static class WslConfigHelper
    {
        /// <summary>
        /// Convert Windows path to WSL path
        /// </summary>
        public static string ConvertToWslPath(string windowsPath)
        {
            if (string.IsNullOrEmpty(windowsPath))
                return windowsPath;
            
            // Replace backslashes with forward slashes
            string path = windowsPath.Replace('\\', '/');
            
            // Convert C:\ to /mnt/c/
            if (path.Length >= 2 && path[1] == ':')
            {
                char driveLetter = char.ToLower(path[0]);
                path = $"/mnt/{driveLetter}/{path.Substring(3)}";
            }
            
            return path;
        }
        
        /// <summary>
        /// Generate a configuration snippet for WSL environments
        /// </summary>
        /// <param name="pythonDir">The Python directory path</param>
        /// <param name="hostIp">The Windows host IP address (optional)</param>
        /// <param name="isVSCode">Whether this is for VSCode configuration</param>
        /// <param name="isWsl">Whether to convert paths for WSL</param>
        /// <returns>JSON configuration object</returns>
        public static object GenerateWslConfig(string pythonDir, string hostIp = null, bool isVSCode = false, bool isWsl = true)
        {
            // Convert Windows path to WSL path if needed
            if (isWsl)
            {
                pythonDir = ConvertToWslPath(pythonDir);
            }
            
            var envVars = new Dictionary<string, string>();
            
            // Add host IP environment variable if provided
            if (!string.IsNullOrEmpty(hostIp))
            {
                envVars["UNITY_HOST"] = hostIp;
            }
            
            var serverConfig = new
            {
                command = "uv",
                args = new[] { "run", "--directory", pythonDir, "server.py" },
                env = envVars.Count > 0 ? envVars : null
            };

            if (isVSCode)
            {
                return new
                {
                    mcp = new
                    {
                        servers = new
                        {
                            unityMCP = serverConfig
                        }
                    }
                };
            }
            else
            {
                return new
                {
                    mcpServers = new
                    {
                        unityMCP = serverConfig
                    }
                };
            }
        }

        /// <summary>
        /// Generate claude mcp add command for WSL
        /// </summary>
        public static string GenerateClaudeAddCommand(string pythonDir, string hostIp = null, bool isWsl = true)
        {
            // Convert Windows path to WSL path if needed
            if (isWsl)
            {
                pythonDir = ConvertToWslPath(pythonDir);
            }
            
            var sb = new StringBuilder();
            sb.Append("claude mcp add unityMCP");
            sb.Append(" --command uv");
            sb.Append($" --args \"run\" \"--directory\" \"{pythonDir}\" \"server.py\"");
            
            if (!string.IsNullOrEmpty(hostIp))
            {
                sb.Append($" --env UNITY_HOST={hostIp}");
            }
            
            return sb.ToString();
        }

        /// <summary>
        /// Generate claude mcp add-json command for WSL
        /// </summary>
        public static string GenerateClaudeAddJsonCommand(string pythonDir, string hostIp = null, bool isWsl = true)
        {
            // Convert Windows path to WSL path if needed
            if (isWsl)
            {
                pythonDir = ConvertToWslPath(pythonDir);
            }
            
            var config = new
            {
                command = "uv",
                args = new[] { "run", "--directory", pythonDir, "server.py" },
                env = !string.IsNullOrEmpty(hostIp) ? new { UNITY_HOST = hostIp } : null
            };
            
            var json = JsonConvert.SerializeObject(config, new JsonSerializerSettings 
            { 
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.None 
            });
            
            return $"claude mcp add-json unityMCP '{json}'";
        }

        /// <summary>
        /// Get common WSL IP addresses for reference
        /// </summary>
        public static List<string> GetCommonWslIps()
        {
            return new List<string>
            {
                "172.30.64.1",  // Common WSL2 default gateway
                "172.16.0.1",   // Alternative WSL2 gateway
                "192.168.0.1",  // Common local network gateway
                "host.docker.internal", // Docker Desktop alias (if available)
                "$(ip route show default | awk '{print $3}')" // Command to get gateway
            };
        }

        /// <summary>
        /// Generate a help text for WSL configuration
        /// </summary>
        public static string GetWslHelpText()
        {
            return @"WSL Configuration Help:

1. Find your Windows host IP from WSL:
   ip route show default | awk '{print $3}'

2. Test connection:
   nc -zv <HOST_IP> 6400

3. Set environment variable (temporary):
   export UNITY_HOST=<HOST_IP>

4. Set permanently in ~/.bashrc:
   echo 'export UNITY_HOST=<HOST_IP>' >> ~/.bashrc

5. Alternative: Use Tailscale IP if available
   (e.g., 100.x.x.x)

Common Issues:
- Windows Firewall blocking port 6400
- WSL2 network changes after reboot
- Multiple network adapters causing confusion";
        }
    }
}