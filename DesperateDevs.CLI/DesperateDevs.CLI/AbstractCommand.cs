namespace DesperateDevs.CLI {

    public abstract class AbstractCommand : ICommand {

        public abstract string trigger { get; }
        public abstract string description { get; }
        public abstract string example { get; }

        protected string[] _rawArgs;
        protected string[] _args;

        public void Run(string[] args) {
            _rawArgs = args;
            _args = args
                .WithoutTrigger()
                .WithoutParameter();
            run();
        }

        protected abstract void run();
    }
}
