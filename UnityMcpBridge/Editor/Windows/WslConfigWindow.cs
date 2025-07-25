using UnityEngine;
using UnityEditor;
using UnityMcpBridge.Editor.Helpers;
using System.IO;

namespace UnityMcpBridge.Editor.Windows
{
    public class WslConfigWindow : EditorWindow
    {
        private string pythonDir;
        private string hostIp = "";
        private Vector2 scrollPosition;
        private bool showAdvanced = false;
        private int selectedIpIndex = 0;
        private string[] commonIps;
        
        [MenuItem("Window/Unity MCP/WSL Configuration")]
        public static void ShowWindow()
        {
            var window = GetWindow<WslConfigWindow>("WSL Configuration");
            window.minSize = new Vector2(500, 400);
            window.Show();
        }

        private void OnEnable()
        {
            pythonDir = FindPackagePythonDirectory();
            var ips = WslConfigHelper.GetCommonWslIps();
            commonIps = ips.ToArray();
        }

        private void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            
            // Title
            EditorGUILayout.LabelField("WSL Configuration for Unity MCP", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            // Help section
            EditorGUILayout.HelpBox(
                "Configure Unity MCP for Windows Subsystem for Linux (WSL) environments.\n" +
                "This helps when running MCP clients in WSL while Unity runs on Windows.",
                MessageType.Info);
            
            EditorGUILayout.Space();
            
            // Python directory
            EditorGUILayout.LabelField("Python Directory", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Windows paths are automatically converted to WSL format", MessageType.None);
            
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.LabelField("Windows Path:", pythonDir);
                EditorGUILayout.LabelField("WSL Path:", WslConfigHelper.ConvertToWslPath(pythonDir));
            }
            
            EditorGUILayout.Space();
            
            // Host IP Configuration
            EditorGUILayout.LabelField("Windows Host IP", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Leave empty for automatic detection, or specify your Windows host IP address.",
                MessageType.None);
            
            EditorGUILayout.BeginHorizontal();
            hostIp = EditorGUILayout.TextField("Host IP:", hostIp);
            
            if (GUILayout.Button("Use Common IP", GUILayout.Width(100)))
            {
                GenericMenu menu = new GenericMenu();
                for (int i = 0; i < commonIps.Length; i++)
                {
                    int index = i;
                    menu.AddItem(new GUIContent(commonIps[i]), false, () => {
                        hostIp = commonIps[index];
                    });
                }
                menu.ShowAsContext();
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            
            // Configuration Commands
            EditorGUILayout.LabelField("Configuration Commands", EditorStyles.boldLabel);
            
            // Claude MCP Add
            if (GUILayout.Button("Copy 'claude mcp add' Command"))
            {
                string command = WslConfigHelper.GenerateClaudeAddCommand(pythonDir, hostIp);
                GUIUtility.systemCopyBuffer = command;
                EditorUtility.DisplayDialog("Command Copied", 
                    "The following command has been copied to clipboard:\n\n" + command, 
                    "OK");
            }
            
            // Claude MCP Add-JSON
            if (GUILayout.Button("Copy 'claude mcp add-json' Command"))
            {
                string command = WslConfigHelper.GenerateClaudeAddJsonCommand(pythonDir, hostIp);
                GUIUtility.systemCopyBuffer = command;
                EditorUtility.DisplayDialog("Command Copied", 
                    "The following command has been copied to clipboard:\n\n" + command, 
                    "OK");
            }
            
            EditorGUILayout.Space();
            
            // Generate JSON Config
            if (GUILayout.Button("Generate JSON Configuration"))
            {
                GenerateJsonConfig();
            }
            
            EditorGUILayout.Space();
            
            // Advanced section
            showAdvanced = EditorGUILayout.Foldout(showAdvanced, "Advanced Help");
            if (showAdvanced)
            {
                EditorGUILayout.HelpBox(WslConfigHelper.GetWslHelpText(), MessageType.None);
                
                if (GUILayout.Button("Copy WSL IP Detection Command"))
                {
                    GUIUtility.systemCopyBuffer = "ip route show default | awk '{print $3}'";
                    EditorUtility.DisplayDialog("Command Copied", 
                        "IP detection command copied to clipboard", "OK");
                }
                
                if (GUILayout.Button("Copy Connection Test Command"))
                {
                    string testCmd = string.IsNullOrEmpty(hostIp) 
                        ? "nc -zv $(ip route show default | awk '{print $3}') 6400"
                        : $"nc -zv {hostIp} 6400";
                    GUIUtility.systemCopyBuffer = testCmd;
                    EditorUtility.DisplayDialog("Command Copied", 
                        "Connection test command copied to clipboard", "OK");
                }
            }
            
            EditorGUILayout.EndScrollView();
        }

        private void GenerateJsonConfig()
        {
            string path = EditorUtility.SaveFilePanel(
                "Save WSL Configuration",
                "",
                "wsl_unity_mcp_config.json",
                "json"
            );

            if (!string.IsNullOrEmpty(path))
            {
                var config = WslConfigHelper.GenerateWslConfig(pythonDir, hostIp, false);
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(config, 
                    Newtonsoft.Json.Formatting.Indented);
                
                File.WriteAllText(path, json);
                EditorUtility.DisplayDialog("Configuration Saved", 
                    $"WSL configuration saved to:\n{path}\n\n" +
                    "You can merge this with your MCP client's configuration file.",
                    "OK");
                
                EditorUtility.RevealInFinder(path);
            }
        }

        private string FindPackagePythonDirectory()
        {
            try
            {
                string packagePath = Path.GetFullPath("Packages/com.justinpbarnett.unity-mcp");
                if (Directory.Exists(packagePath))
                {
                    // For embedded package
                    string pythonPath = Path.Combine(packagePath, "..", "..", "UnityMcpServer", "src");
                    pythonPath = Path.GetFullPath(pythonPath);
                    if (Directory.Exists(pythonPath))
                    {
                        return pythonPath;
                    }
                }

                // Check Library/PackageCache for registry packages
                string[] packageCachePaths = Directory.GetDirectories(
                    Path.Combine(Application.dataPath, "..", "Library", "PackageCache"),
                    "com.justinpbarnett.unity-mcp*",
                    SearchOption.TopDirectoryOnly
                );

                if (packageCachePaths.Length > 0)
                {
                    string pythonPath = Path.Combine(packageCachePaths[0], "..", "..", "..", "..", "UnityMcpServer", "src");
                    pythonPath = Path.GetFullPath(pythonPath);
                    if (Directory.Exists(pythonPath))
                    {
                        return pythonPath;
                    }
                }

                // Check standard installation paths
#if UNITY_EDITOR_WIN
                string userPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
                string installPath = Path.Combine(userPath, "Programs", "UnityMCP", "UnityMcpServer", "src");
#elif UNITY_EDITOR_OSX
                string installPath = "/usr/local/bin/UnityMCP/UnityMcpServer/src";
#else
                string installPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile), "UnityMCP", "UnityMcpServer", "src");
#endif
                if (Directory.Exists(installPath))
                {
                    return installPath;
                }

                return "<Python directory not found>";
            }
            catch
            {
                return "<Error finding Python directory>";
            }
        }
    }
}