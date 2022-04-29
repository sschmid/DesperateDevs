using System.Linq;

namespace DesperateDevs.Cli.Utils
{
    public abstract class AbstractCommand : ICommand
    {
        public abstract string Trigger { get; }
        public abstract string Description { get; }
        public abstract string Group { get; }
        public abstract string Example { get; }

        protected CliProgram _program;
        protected string[] _rawArgs;
        protected string[] _args;

        public virtual void Run(CliProgram program, string[] args)
        {
            _program = program;
            _rawArgs = args;
            _args = args
                .WithoutTrigger()
                .WithoutDefaultParameter()
                .ToArray();

            Run();
        }

        protected abstract void Run();
    }
}
