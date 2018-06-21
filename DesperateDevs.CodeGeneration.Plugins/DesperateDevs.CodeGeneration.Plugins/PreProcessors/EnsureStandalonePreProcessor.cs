// Deprecated

//using System;
//using System.Linq;
//using DesperateDevs.Utils;
//
//namespace DesperateDevs.CodeGeneration.Plugins {
//
//    public class EnsureStandalonePreProcessor : IPreProcessor {
//
//        public string name { get { return "Ensure Standalone"; } }
//        public int priority { get { return -5; } }
//        public bool runInDryMode { get { return true; } }
//
//        public void PreProcess() {
//            var types = AppDomain.CurrentDomain.GetAllTypes();
//            var isStandalone = types.Any(type => type.FullName == "DesperateDevs.CodeGeneration.CodeGenerator.CLI.Program");
//            if (!isStandalone) {
//                throw new Exception("Jenny is configured to only work with the standalone code generator!\n" +
//                                    "You can remove this error by removing '" + name + "' from the Pre Processors.");
//            }
//        }
//    }
//}
