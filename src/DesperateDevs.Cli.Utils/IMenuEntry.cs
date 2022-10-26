using System;

namespace DesperateDevs.Cli.Utils
{
    public interface IMenuEntry
    {
        string Title { get; }
        ConsoleKey[] Triggers { get; }
        bool ShowTriggerInTitle { get; }
        Action Action { get; }
    }
}
