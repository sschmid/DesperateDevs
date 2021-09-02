namespace DesperateDevs.CLI.Utils {

    public interface ICommand {

        string trigger { get; }
        string description { get; }
        string group { get; }
        string example { get; }

        void Run(CLIProgram program, string[] args);
    }
}
