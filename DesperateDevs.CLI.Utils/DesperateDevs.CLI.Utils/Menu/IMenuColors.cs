using System;

namespace DesperateDevs.CLI.Utils
{
    public interface IMenuColors
    {
        ConsoleColor normalBackground { get; }
        ConsoleColor normalForeground { get; }
        ConsoleColor selectedBackground { get; }
        ConsoleColor selectedForeground { get; }
        ConsoleColor disabledBackground { get; }
        ConsoleColor disabledForeground { get; }
    }
}
