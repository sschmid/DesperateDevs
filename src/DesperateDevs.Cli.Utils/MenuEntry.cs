using System;

namespace DesperateDevs.Cli.Utils
{
    public class MenuEntry : IMenuEntry
    {
        public string Title { get; set; }
        public ConsoleKey[] Triggers { get; set; }
        public bool ShowTriggerInTitle { get; set; }
        public Action Action { get; set; }

        public MenuEntry(string title, ConsoleKey[] triggers, bool showTriggerInTitle, Action action)
        {
            Title = title;
            Triggers = triggers;
            ShowTriggerInTitle = showTriggerInTitle;
            Action = action;
        }
    }
}
