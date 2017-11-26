namespace DesperateDevs.CLI {

    public interface ICommand {

        string trigger { get; }
        string description { get; }
		string example { get; }

        void Run(string[] args);
    }
}
