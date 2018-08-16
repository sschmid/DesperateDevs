using System;

namespace DesperateDevs.CLI.Utils
{
    public sealed class DefaultMenuColors : IMenuColors
    {
        public ConsoleColor normalBackground { get { return ConsoleColor.Black; } }
        public ConsoleColor normalForeground { get { return ConsoleColor.White; } }
        public ConsoleColor selectedBackground { get { return ConsoleColor.White; } }
        public ConsoleColor selectedForeground { get { return ConsoleColor.Black; } }
        public ConsoleColor disabledBackground { get { return ConsoleColor.Gray; } }
        public ConsoleColor disabledForeground { get { return ConsoleColor.Black; } }
    }
}
