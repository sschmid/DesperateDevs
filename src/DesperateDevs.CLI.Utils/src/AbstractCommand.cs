namespace DesperateDevs.CLI.Utils
{
    public abstract class AbstractCommand : ICommand
    {
        public abstract string trigger { get; }
        public abstract string description { get; }
        public abstract string group { get; }
        public abstract string example { get; }

        protected CLIProgram _program;
        protected string[] _rawArgs;
        protected string[] _args;

        public virtual void Run(CLIProgram program, string[] args)
        {
            _program = program;
            _rawArgs = args;
            _args = args
                .WithoutTrigger()
                .WithoutDefaultParameter();

            run();
        }

        protected abstract void run();
    }
}
