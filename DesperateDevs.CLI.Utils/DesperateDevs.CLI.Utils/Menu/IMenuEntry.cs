using System;

namespace DesperateDevs.CLI.Utils
{
    public interface IMenuEntry
    {
        string title { get; }
        ConsoleKey? trigger { get; }
        bool showTriggerInTitle { get; }
        Action action { get; }
    }
}
