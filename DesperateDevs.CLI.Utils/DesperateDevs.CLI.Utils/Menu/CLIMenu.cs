using System;
using System.Collections.Generic;

namespace DesperateDevs.CLI.Utils
{
    public class CLIMenu
    {
        public string title { get { return _title; } }

        readonly string _title;
        readonly MenuSelection _selection;
        readonly List<IMenuEntry> _menuEntries;
        readonly IMenuColors _colors;

        bool _stopRequested;

        public CLIMenu(string title, IMenuColors colors)
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
            _stopRequested = false;
            while (!_stopRequested)
            {
                drawMenu();
                processInput();
                tryRunMenuEntry();
                _selection.runAction = false;
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
                if (i == _selection.index)
                {
                    Console.BackgroundColor = _colors.selectedBackground;
                    Console.ForegroundColor = _colors.selectedForeground;
                }
                else
                {
                    Console.BackgroundColor = _colors.normalBackground;
                    Console.ForegroundColor = _colors.normalForeground;
                }

                if (_menuEntries[i].showTriggerInTitle)
                    Console.WriteLine("[" + _menuEntries[i].trigger + "] " + _menuEntries[i].title);
                else
                    Console.WriteLine(_menuEntries[i].title);

                Console.ResetColor();
            }
        }

        void tryRunMenuEntry()
        {
            if (_selection.runAction)
                _menuEntries[_selection.index].action();
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
                case ConsoleKey.Enter:
                    _selection.runAction = true;
                    break;
            }
        }

        void selectionFromTrigger(ConsoleKeyInfo key)
        {
            for (var i = 0; i < _menuEntries.Count; i++)
            {
                if (_menuEntries[i].trigger == key.Key)
                {
                    _selection.index = i;
                    _selection.runAction = true;
                    return;
                }
            }
        }
    }
}
