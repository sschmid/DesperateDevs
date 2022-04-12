namespace DesperateDevs.CLI.Utils
{
    public abstract class AbstractCommand : ICommand
    {
        public abstract string Trigger { get; }
        public abstract string Description { get; }
        public abstract string Group { get; }
        public abstract string Example { get; }

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

            Run();
        }

        protected abstract void Run();
    }
}
