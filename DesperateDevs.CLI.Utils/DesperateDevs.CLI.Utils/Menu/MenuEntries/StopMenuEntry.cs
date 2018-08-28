using System;

namespace DesperateDevs.CLI.Utils
{
    public sealed class StopMenuEntry : MenuEntry
    {
        public StopMenuEntry(CLIMenu menu, string title, bool showTriggerInTitle) :
            base(title, new[] { ConsoleKey.Escape, ConsoleKey.Q }, showTriggerInTitle, menu.Stop)
        {
        }
    }
}
