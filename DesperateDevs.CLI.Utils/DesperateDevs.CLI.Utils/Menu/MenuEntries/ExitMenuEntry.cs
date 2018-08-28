﻿using System;

namespace DesperateDevs.CLI.Utils
{
    public sealed class ExitMenuEntry : MenuEntry
    {
        public ExitMenuEntry(string title, bool showTriggerInTitle) :
            base(title, new[] { ConsoleKey.Escape, ConsoleKey.Q }, showTriggerInTitle, () => Environment.Exit(0))
        {
        }
    }
}
