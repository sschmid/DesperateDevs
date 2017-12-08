﻿using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using DesperateDevs.Networking;
using DesperateDevs.Serialization;
using DesperateDevs.Utils;
using UnityEditor;
using UnityEngine;

namespace DesperateDevs.CodeGeneration.CodeGenerator.Unity.Editor {

    public static class UnityCodeGenerator {

        [MenuItem("Tools/Jenny/Generate #%g", false, 100)]
        public static void Generate() {
            checkCanGenerate();

            Debug.Log("Generating...");

            var codeGenerator = CodeGeneratorUtil.CodeGeneratorFromPreferences(Preferences.sharedInstance);

            var progressOffset = 0f;

            codeGenerator.OnProgress += (title, info, progress) => {
                var cancel = EditorUtility.DisplayCancelableProgressBar(title, info, progressOffset + progress / 2);
                if (cancel) {
                    codeGenerator.Cancel();
                }
            };

            CodeGenFile[] dryFiles = null;
            CodeGenFile[] files = null;

            try {
                dryFiles = codeGenerator.DryRun();
                progressOffset = 0.5f;
                files = codeGenerator.Generate();
            } catch (Exception ex) {
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

        static void checkCanGenerate() {
            if (EditorApplication.isCompiling) {
                throw new Exception("Cannot generate because Unity is still compiling. Please wait...");
            }

            var assembly = typeof(UnityEditor.Editor).Assembly;

            var logEntries = assembly.GetType("UnityEditorInternal.LogEntries")
                             ?? assembly.GetType("UnityEditor.LogEntries");

            logEntries.GetMethod("Clear").Invoke(new object(), null);
            var canCompile = (int)logEntries.GetMethod("GetCount").Invoke(new object(), null) == 0;
            if (!canCompile) {
                Debug.Log("There are compile errors! Generated code will be based on last compiled executable.");
                EditorUtility.DisplayDialog("Jenny",
                    "There are compile errors! Generated code will be based on last compiled executable.",
                    "Ok"
                );
            }
        }

        [MenuItem("Tools/Jenny/Generate with external Code Generator %&g", false, 101)]
        public static void GenerateExternal() {
            Debug.Log("Connecting...");

            var config = Preferences.sharedInstance.CreateAndConfigure<CodeGeneratorConfig>();
            var client = new TcpClientSocket();
            client.OnConnected += onConnected;
            client.OnReceived += onReceive;
            client.OnDisconnected += onDisconnect;
            client.Connect(config.host.ResolveHost(), config.port);

            AssetDatabase.Refresh();
        }

        static void onConnected(TcpClientSocket client) {
            Debug.Log("Connected");
            Debug.Log("Generating...");
            client.Send(Encoding.UTF8.GetBytes("gen"));
        }

        static void onReceive(AbstractTcpSocket socket, Socket client, byte[] bytes) {
            Debug.Log("Generated");
            socket.Disconnect();
        }

        static void onDisconnect(AbstractTcpSocket socket) {
            Debug.Log("Disconnected");
        }
    }
}