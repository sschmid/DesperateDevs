using System;
using System.Collections.Generic;
using System.Linq;

namespace DesperateDevs.Cli.Utils
{
    public class CLIMenu
    {
        public string title => _title;
        public string indent = string.Empty;

        readonly string _title;
        readonly MenuSelection _selection;
        readonly List<IMenuEntry> _menuEntries;
        readonly ConsoleColors _colors;

        int _longestTitle;
        bool _stopRequested;

        public CLIMenu(string title, ConsoleColors colors)
        {
            _title = title;
            _selection = new MenuSelection();
            _menuEntries = new List<IMenuEntry>();
            _colors = colors;
        }

        public void AddMenuEntry(IMenuEntry entry)
        {
            _menuEntries.Add(entry);
        }

        public void Start()
        {
            _longestTitle = _menuEntries.Max(e => e.title.Length) + indent.Length;
            _stopRequested = false;
            while (!_stopRequested)
            {
                drawMenu();
                processInput();
                tryRunMenuEntry();
            }

            Console.Clear();
        }

        public void Stop()
        {
            _stopRequested = true;
        }

        void drawMenu()
        {
            Console.Clear();
            Console.WriteLine(_title);

            for (int i = 0; i < _menuEntries.Count; i++)
            {
                var entry = indent;
                if (_menuEntries[i].showTriggerInTitle)
                {
                    entry += _menuEntries[i].triggers != null
                        ? "[" + _menuEntries[i].triggers[0] + "] "
                        : string.Empty;
                }

                entry += _menuEntries[i].title;

                if (i == _selection.index)
                {
                    CLIHelper.WriteHighlighted(entry, true, _longestTitle);
                }
                else
                {
                    Console.ResetColor();
                    Console.WriteLine(entry);
                }
            }
        }

        void tryRunMenuEntry()
        {
            if (_selection.runAction)
                _menuEntries[_selection.index].action();

            _selection.runAction = false;
        }

        void processInput()
        {
            var key = Console.ReadKey(true);
            selectionFromNavigation(key);
            selectionFromTrigger(key);
        }

        void selectionFromNavigation(ConsoleKeyInfo key)
        {
            switch (key.Key)
            {
                case ConsoleKey.DownArrow:
                case ConsoleKey.RightArrow:
                case ConsoleKey.J:
                case ConsoleKey.L:
                    if (_selection.index < _menuEntries.Count - 1)
                    {
                        _selection.index += 1;
                        _selection.runAction = false;
                    }
                    break;
                case ConsoleKey.UpArrow:
                case ConsoleKey.LeftArrow:
                case ConsoleKey.K:
                case ConsoleKey.H:
                    if (_selection.index > 0)
                    {
                        _selection.index -= 1;
                        _selection.runAction = false;
                    }
                    break;
                case ConsoleKey.Home:
                case ConsoleKey.A:
                    _selection.index = 0;
                    _selection.runAction = false;
                    break;
                case ConsoleKey.End:
                case ConsoleKey.E:
                    _selection.index = _menuEntries.Count - 1;
                    _selection.runAction = false;
                    break;
                case ConsoleKey.Enter:
                case ConsoleKey.Spacebar:
                    _selection.runAction = true;
                    break;
            }
        }

        void selectionFromTrigger(ConsoleKeyInfo key)
        {
            for (var i = 0; i < _menuEntries.Count; i++)
            {
                if (_menuEntries[i].triggers != null)
                {
                    foreach (var trigger in _menuEntries[i].triggers)
                    {
                        if (trigger == key.Key)
                        {
                            _selection.index = i;
                            _selection.runAction = true;
                            return;
                        }
                    }
                }
            }
        }
    }
}
