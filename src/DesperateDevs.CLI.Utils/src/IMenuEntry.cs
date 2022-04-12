using System;

namespace DesperateDevs.Cli.Utils
{
    public interface IMenuEntry
    {
        string title { get; }
        ConsoleKey[] triggers { get; }
        bool showTriggerInTitle { get; }
        Action action { get; }
    }
}
