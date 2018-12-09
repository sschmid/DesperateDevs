using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using DesperateDevs.Networking;
using DesperateDevs.Serialization;
using DesperateDevs.Utils;
using UnityEditor;
using UnityEngine;

namespace DesperateDevs.CodeGeneration.CodeGenerator.Unity.Editor
{
    public static class UnityCodeGenerator
    {
        public const string DRY_RUN = "DesperateDevs.CodeGeneration.CodeGenerator.Unity.Editor.DryRun";

        public static Preferences GetPreferences()
        {
            var propertiesPath = EditorPrefs.GetString(CodeGeneratorPreferencesDrawer.PROPERTIES_PATH_KEY, CodeGenerator.defaultPropertiesPath);
            return new Preferences(propertiesPath, Preferences.defaultUserPropertiesPath);
        }

        [MenuItem(CodeGeneratorMenuItems.generate, false, CodeGeneratorMenuItemPriorities.generate)]
        public static void Generate()
        {
            Debug.Log("Generating...");

            var codeGenerator = CodeGeneratorUtil.CodeGeneratorFromPreferences(GetPreferences());

            var progressOffset = 0f;

            codeGenerator.OnProgress += (title, info, progress) =>
            {
                var cancel = EditorUtility.DisplayCancelableProgressBar(title, info, progressOffset + progress / 2);
                if (cancel)
                {
                    codeGenerator.Cancel();
                }
            };

            CodeGenFile[] dryFiles = null;
            CodeGenFile[] files = null;

            try
            {
                dryFiles = EditorPrefs.GetBool(DRY_RUN, true) ? codeGenerator.DryRun() : new CodeGenFile[0];
                progressOffset = 0.5f;
                files = codeGenerator.Generate();
            }
            catch (Exception ex)
            {
                dryFiles = new CodeGenFile[0];
                files = new CodeGenFile[0];

                EditorUtility.DisplayDialog("Error", ex.Message, "Ok");
            }

            EditorUtility.ClearProgressBar();

            var totalGeneratedFiles = files.Select(file => file.fileName).Distinct().Count();

            var sloc = dryFiles
                .Select(file => file.fileContent.ToUnixLineEndings())
                .Sum(content => content.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).Length);

            var loc = files
                .Select(file => file.fileContent.ToUnixLineEndings())
                .Sum(content => content.Split(new[] { '\n' }).Length);

            Debug.Log("Generated " + totalGeneratedFiles + " files (" + sloc + " sloc, " + loc + " loc)");

            AssetDatabase.Refresh();
        }

        static string _propertiesPath;

        [MenuItem(CodeGeneratorMenuItems.generate_server, false, CodeGeneratorMenuItemPriorities.generate_server)]
        public static void GenerateExternal()
        {
            Debug.Log("Connecting...");

            var preferences = GetPreferences();
            _propertiesPath = preferences.propertiesPath;
            var config = preferences.CreateAndConfigure<CodeGeneratorConfig>();
            var client = new TcpClientSocket();
            client.OnConnected += onConnected;
            client.OnReceived += onReceive;
            client.OnDisconnected += onDisconnect;
            client.Connect(config.host.ResolveHost(), config.port);
        }

        static void onConnected(TcpClientSocket client)
        {
            Debug.Log("Connected");
            Debug.Log("Generating...");
            client.Send(Encoding.UTF8.GetBytes("gen " + _propertiesPath));
        }

        static void onReceive(AbstractTcpSocket socket, Socket client, byte[] bytes)
        {
            Debug.Log("Generated");
            socket.Disconnect();
        }

        static void onDisconnect(AbstractTcpSocket socket)
        {
            Debug.Log("Disconnected");
        }
    }
}
