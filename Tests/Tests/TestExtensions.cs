using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using NSpec;
using Shouldly;

public static class TestExtensions
{
    public static string ProjectName;

    public static void Fail(this nspec spec)
    {
        "but did".ShouldBe("should not happen");
    }

    public static void Wait(this nspec spec, int ms = 50)
    {
        Thread.Sleep(ms);
    }

    public static string GetProjectRoot()
    {
        if (string.IsNullOrEmpty(ProjectName))
            throw new Exception("TestExtensions.ProjectName is not set!");

        var current = new DirectoryInfo(Directory.GetCurrentDirectory());
        while (current.Name != ProjectName) current = current.Parent;
        return current.FullName;
    }

    public static Dictionary<string, string> GetSourceFiles(string path)
    {
        return Directory.GetFiles(path, "*.cs", SearchOption.AllDirectories)
            .Where(p =>
                !p.Contains(dir("Generated")) &&
                !p.Contains(dir("Libraries")) &&
                !p.Contains(dir("Tests")) &&
                !p.Contains(dir("TestFixtures")) &&
                !p.Contains(dir("Fixtures")) &&
                !p.Contains(dir("TestDependency")) &&
                !p.Contains(dir("TestDependencyBase")) &&
                !p.Contains(dir("TestUpdateCSProjPostProcessor")) &&
                !p.Contains(dir("Examples")) &&
                !p.Contains(dir("Readme")) &&
                !p.Contains(dir("Build")) &&
                !p.Contains(dir("bin")) &&
                !p.Contains(dir("obj")) &&
                !p.Contains(dir("packages")) &&
                !p.Contains("AssemblyInfo.cs") &&
                !p.Contains("Commands.cs") &&
                !p.Contains("Program.cs")
            ).ToDictionary(p => p, File.ReadAllText);
    }

    public static Dictionary<string, string> GetSourceFilesInclAllProjects(string path)
    {
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

    static string dir(params string[] paths)
    {
        return paths.Aggregate(string.Empty, (pathString, p) => pathString + p + Path.DirectorySeparatorChar);
    }
}
