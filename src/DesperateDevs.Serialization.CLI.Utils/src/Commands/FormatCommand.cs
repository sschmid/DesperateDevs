﻿using DesperateDevs.CLI.Utils;

namespace DesperateDevs.Serialization.CLI.Utils
{
    public class FormatCommand : AbstractPreferencesCommand
    {
        public override string Trigger => "format";
        public override string Description => "Format the config files";
        public override string Group => CommandGroups.PROPERTIES;
        public override string Example => "format [-mini]";

        public FormatCommand() : base(typeof(FormatCommand).FullName)
        {
        }

        protected override void Run()
        {
            _preferences.Save(_args.HasParameter("-mini"));
        }
    }
}
