using System;
using System.Collections.Generic;
using System.Linq;

namespace DesperateDevs.Cli.Utils
{
    public class CliMenu
    {
        public string Title => _title;
        public string Indent = string.Empty;

        readonly string _title;
        readonly MenuSelection _selection;
        readonly List<IMenuEntry> _menuEntries;

        int _longestTitle;
        bool _stopRequested;

        public CliMenu(string title)
        {
            _title = title;
            _selection = new MenuSelection();
            _menuEntries = new List<IMenuEntry>();
        }

        public void AddMenuEntry(IMenuEntry entry) => _menuEntries.Add(entry);

        public void Start()
        {
            _longestTitle = _menuEntries.Max(e => e.Title.Length) + Indent.Length;
            _stopRequested = false;
            while (!_stopRequested)
            {
                DrawMenu();
                ProcessInput();
                TryRunMenuEntry();
            }

            Console.Clear();
        }

        public void Stop() => _stopRequested = true;

        void DrawMenu()
        {
            Console.Clear();
            Console.WriteLine(_title);

            for (var i = 0; i < _menuEntries.Count; i++)
            {
                var entry = Indent;
                var menuEntry = _menuEntries[i];
                if (menuEntry.ShowTriggerInTitle)
                {
                    entry += menuEntry.Triggers != null
                        ? $"[{menuEntry.Triggers[0]}] "
                        : string.Empty;
                }

                entry += menuEntry.Title;

                if (i == _selection.Index)
                    CliHelper.WriteHighlighted(entry, true, _longestTitle);
                else
                    Console.WriteLine(entry);
            }
        }

        void ProcessInput()
        {
            var key = Console.ReadKey(true);
            SelectionFromNavigation(key);
            SelectionFromTrigger(key);
        }

        void SelectionFromNavigation(ConsoleKeyInfo key)
        {
            switch (key.Key)
            {
                case ConsoleKey.DownArrow:
                case ConsoleKey.RightArrow:
                case ConsoleKey.J:
                case ConsoleKey.L:
                    if (_selection.Index < _menuEntries.Count - 1)
                    {
                        _selection.Index += 1;
                        _selection.RunAction = false;
                    }

                    break;
                case ConsoleKey.UpArrow:
                case ConsoleKey.LeftArrow:
                case ConsoleKey.K:
                case ConsoleKey.H:
                    if (_selection.Index > 0)
                    {
                        _selection.Index -= 1;
                        _selection.RunAction = false;
                    }

                    break;
                case ConsoleKey.Home:
                case ConsoleKey.A:
                    _selection.Index = 0;
                    _selection.RunAction = false;
                    break;
                case ConsoleKey.End:
                case ConsoleKey.E:
                    _selection.Index = _menuEntries.Count - 1;
                    _selection.RunAction = false;
                    break;
                case ConsoleKey.Enter:
                case ConsoleKey.Spacebar:
                    _selection.RunAction = true;
                    break;
            }
        }

        void SelectionFromTrigger(ConsoleKeyInfo key)
        {
            for (var i = 0; i < _menuEntries.Count; i++)
            {
                var menuEntry = _menuEntries[i];
                if (menuEntry.Triggers != null)
                {
                    if (menuEntry.Triggers.Any(trigger => trigger == key.Key))
                    {
                        _selection.Index = i;
                        _selection.RunAction = true;
                        return;
                    }
                }
            }
        }

        void TryRunMenuEntry()
        {
            if (_selection.RunAction)
            {
                _selection.RunAction = false;
                _menuEntries[_selection.Index].Action();
            }
        }
    }
}
