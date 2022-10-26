using System;
using System.Threading;
using DesperateDevs.Cli.Utils;

namespace DesperateDevs.Cli.Utils.Program
{
    static class Program
    {
        public static void Main(string[] args)
        {
            CliHelper.ConsoleColors = new ConsoleColors();
            var menu = new CliMenu("Cli Tester");
            menu.AddMenuEntry(new GreetMenuEntry());
            menu.AddMenuEntry(new SubMenuEntry(menu.Title));
            menu.AddMenuEntry(new SelectionMenuEntry(menu.Title));
            menu.AddMenuEntry(new StopMenuEntry(menu, "Exit", true));
            menu.Start();
            Console.Clear();
        }
    }
}

public sealed class GreetMenuEntry : IMenuEntry
{
    public string Title => "Greet";
    public ConsoleKey[] Triggers => new[] {ConsoleKey.G};
    public bool ShowTriggerInTitle => true;

    public Action Action => () =>
    {
        Console.WriteLine($"Hello {new Random().Next()}");
        Thread.Sleep(1000);
    };
}

public sealed class SubMenuEntry : IMenuEntry
{
    public string Title => "Sub menu";
    public ConsoleKey[] Triggers => new[] {ConsoleKey.S};
    public bool ShowTriggerInTitle => true;

    readonly string _subtitle;

    public SubMenuEntry(string previous)
    {
        _subtitle = $"{previous} > {Title}";
    }

    public Action Action => () =>
    {
        var menu = new CliMenu(_subtitle);
        menu.AddMenuEntry(new GreetMenuEntry());
        menu.AddMenuEntry(new SubMenuEntry(_subtitle));
        menu.AddMenuEntry(new StopMenuEntry(menu, "Back", true));
        menu.Start();
    };
}

public sealed class SelectionMenuEntry : IMenuEntry
{
    public string Title => "Selection";
    public ConsoleKey[] Triggers => new[] {ConsoleKey.X};
    public bool ShowTriggerInTitle => true;

    readonly string _subtitle;

    public SelectionMenuEntry(string previous)
    {
        _subtitle = $"{previous} > {Title}";
    }

    public Action Action => () =>
    {
        var menu = new CliMenu(_subtitle);
        menu.AddMenuEntry(new SelectableMenuEntry("Desperate", false));
        menu.AddMenuEntry(new SelectableMenuEntry("Devs", false));
        menu.AddMenuEntry(new SelectableMenuEntry("Console", false));
        menu.AddMenuEntry(new SelectableMenuEntry("Menu", false));
        menu.AddMenuEntry(new StopMenuEntry(menu, "Back", true));
        menu.Start();
    };
}
