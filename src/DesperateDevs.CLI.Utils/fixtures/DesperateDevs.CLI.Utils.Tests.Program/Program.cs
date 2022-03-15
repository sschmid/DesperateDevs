using System;
using System.Threading;
using DesperateDevs.CLI.Utils;
using DesperateDevs;

namespace DesperateDevs.CLI.Utils.Tests.Program
{
    static class Program
    {
        public static void Main(string[] args)
        {
            CLIHelper.consoleColors = new ConsoleColors();
            var menu = new CLIMenu("CLI Tester", CLIHelper.consoleColors);
            menu.AddMenuEntry(new GreetMenuEntry());
            menu.AddMenuEntry(new SubMenuEntry(menu.title));
            menu.AddMenuEntry(new SelectionMenuEntry(menu.title));
            menu.AddMenuEntry(new StopMenuEntry(menu, "Exit", true));
            menu.Start();
            Console.Clear();
        }
    }
}

public sealed class GreetMenuEntry : IMenuEntry
{
    public string title => "Greet";
    public ConsoleKey[] triggers => new[] {ConsoleKey.G};
    public bool showTriggerInTitle => true;

    public Action action => () =>
    {
        Console.WriteLine("Hello " + new Random().Next());
        Thread.Sleep(1000);
    };
}

public sealed class SubMenuEntry : IMenuEntry
{
    public string title => "Sub menu";
    public ConsoleKey[] triggers => new[] {ConsoleKey.S};
    public bool showTriggerInTitle => true;

    readonly string _subtitle;

    public SubMenuEntry(string previous)
    {
        _subtitle = previous + " > " + title;
    }

    public Action action => () =>
    {
        var menu = new CLIMenu(_subtitle, CLIHelper.consoleColors);
        menu.AddMenuEntry(new GreetMenuEntry());
        menu.AddMenuEntry(new SubMenuEntry(_subtitle));
        menu.AddMenuEntry(new StopMenuEntry(menu, "Back", true));
        menu.Start();
    };
}

public sealed class SelectionMenuEntry : IMenuEntry
{
    public string title => "Selection";
    public ConsoleKey[] triggers => new[] {ConsoleKey.X};
    public bool showTriggerInTitle => true;

    readonly string _subtitle;

    public SelectionMenuEntry(string previous)
    {
        _subtitle = previous + " > " + title;
    }

    public Action action => () =>
    {
        var menu = new CLIMenu(_subtitle, CLIHelper.consoleColors);
        menu.AddMenuEntry(new SelectableMenuEntry("Desperate", false));
        menu.AddMenuEntry(new SelectableMenuEntry("Devs", false));
        menu.AddMenuEntry(new SelectableMenuEntry("Console", false));
        menu.AddMenuEntry(new SelectableMenuEntry("Menu", false));
        menu.AddMenuEntry(new StopMenuEntry(menu, "Back", true));
        menu.Start();
    };
}
