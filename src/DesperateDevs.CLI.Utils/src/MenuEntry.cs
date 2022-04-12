using System;

namespace DesperateDevs.Cli.Utils
{
    public class MenuEntry : IMenuEntry
    {
        public string title { get; set; }
        public ConsoleKey[] triggers { get; set; }
        public bool showTriggerInTitle { get; set; }
        public Action action { get; set; }

        public MenuEntry(string title, ConsoleKey[] triggers, bool showTriggerInTitle, Action action)
        {
            this.title = title;
            this.triggers = triggers;
            this.showTriggerInTitle = showTriggerInTitle;
            this.action = action;
        }
    }
}
