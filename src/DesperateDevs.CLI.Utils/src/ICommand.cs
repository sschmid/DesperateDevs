﻿namespace DesperateDevs.Cli.Utils
{
    public interface ICommand
    {
        string Trigger { get; }
        string Description { get; }
        string Group { get; }
        string Example { get; }

        void Run(CLIProgram program, string[] args);
    }
}
