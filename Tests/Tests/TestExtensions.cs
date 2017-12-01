﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using DesperateDevs.Logging;
using DesperateDevs.Logging.Appenders;
using DesperateDevs.Logging.Formatters;
using NSpec;

public static class TestExtensions {

    [Obsolete("Just for testing. Please remove when done.")]
    public static void SetupLogging(this nspec spec) {
        var appender = new ConsoleAppender();
        var formatter = new DefaultLogMessageFormatter();

        fabl.AddAppender((logger, level, message) => {
            var msg = formatter.FormatMessage(logger, level, message);
            appender.WriteLine(logger, level, msg);
        });
    }

    public static void Fail(this nspec spec) {
        "but did".should_be("should not happen");
    }

    public static void Wait(this nspec spec, int ms = 50) {
        Thread.Sleep(ms);
    }

    public static string GetProjectRoot() {
        var current = new DirectoryInfo(Directory.GetCurrentDirectory());
        if (current.Parent.Parent.Name == "Tests") {
            // This happens if you run the TestRunner from your IDE
            return current.Parent.Parent.Parent.FullName;
        }

        if (current.Name == "Scripts") {
            // This happens if you run ./run_tests
            return current.Parent.FullName;
        }

        // This happens if you run ./Scripts/run_tests
        return current.FullName;
    }

    public static Dictionary<string, string> GetSourceFiles(string path) {
        return Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories)
            .Where(p =>
                !p.Contains(dir("Generated")) &&
                !p.Contains(dir("Libraries")) &&
                !p.Contains(dir("Tests")) &&
                !p.Contains(dir("Tests.Fixtures")) &&
                !p.Contains(dir("Examples")) &&
                !p.Contains(dir("Readme")) &&
                !p.Contains(dir("Build")) &&
                !p.Contains(dir("bin")) &&
                !p.Contains(dir("obj")) &&
                !p.Contains("AssemblyInfo.cs") &&
                !p.Contains("Commands.cs") &&
                !p.Contains("Program.cs")
            ).ToDictionary(p => p, p => File.ReadAllText(p));
    }

    public static Dictionary<string, string> GetSourceFilesInclAllProjects(string path) {
        return Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories)
            .Where(p =>
                !p.Contains(dir("Generated")) &&
                !p.Contains(dir("Libraries")) &&
                !p.Contains(dir("Build")) &&
                !p.Contains(dir("bin")) &&
                !p.Contains(dir("obj")) &&
                !p.Contains("AssemblyInfo.cs")
            ).ToDictionary(p => p, p => File.ReadAllText(p));
    }

    static string dir(params string[] paths) {
        return paths.Aggregate(string.Empty, (pathString, p) => pathString + p + Path.DirectorySeparatorChar);
    }
}
