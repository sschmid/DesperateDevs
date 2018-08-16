using System;

namespace DesperateDevs.CLI.Utils
{
    public sealed class StopMenuEntry : IMenuEntry
    {
        public string title { get { return _stopLabel; } }
        public ConsoleKey? trigger { get { return ConsoleKey.Escape; } }
        public bool showTriggerInTitle { get { return true; } }

        readonly CLIMenu _menu;
        readonly string _stopLabel;

        public StopMenuEntry(CLIMenu menu, string stopLabel)
        {
            _menu = menu;
            _stopLabel = stopLabel;
        }

        public Action action { get { return _menu.Stop; } }
    }
}
