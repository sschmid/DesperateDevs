using System;

namespace DesperateDevs.CLI.Utils
{
    public class MenuEntry : IMenuEntry
    {
        public string title { get; set; }
        public ConsoleKey? trigger { get; set; }
        public bool showTriggerInTitle { get; set; }
        public Action action { get; set; }

        public MenuEntry(string title, ConsoleKey? trigger, bool showTriggerInTitle, Action action)
        {
            this.title = title;
            this.trigger = trigger;
            this.showTriggerInTitle = showTriggerInTitle;
            this.action = action;
        }
    }
}
