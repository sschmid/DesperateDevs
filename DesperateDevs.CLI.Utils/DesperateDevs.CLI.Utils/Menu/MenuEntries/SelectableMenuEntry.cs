using System;

namespace DesperateDevs.CLI.Utils
{
    public class SelectableMenuEntry : IMenuEntry
    {
        public string title { get { return (_selected ? "[x] " : "[ ] ") + _title; } }
        public ConsoleKey? trigger { get { return null; } }
        public Action action { get { return toggleSelection; } }
        public bool showTriggerInTitle { get { return false; } }

        public bool selected { get { return _selected; } }

        readonly string _title;

        bool _selected;

        public SelectableMenuEntry(string title)
        {
            _title = title;
        }

        void toggleSelection()
        {
            _selected = !_selected;
        }
    }
}
