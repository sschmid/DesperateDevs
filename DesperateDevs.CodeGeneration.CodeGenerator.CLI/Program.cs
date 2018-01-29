using System;
using DesperateDevs.CLI.Utils;

namespace DesperateDevs.CodeGeneration.CodeGenerator.CLI {

    class Program {

        static CLIProgram _program;

        public static void Main(string[] args) {
            _program = new CLIProgram("Jenny", args, printUsage);
            _program.Run();
        }

        public static ICommand GetCommand(string trigger) {
            return _program.GetCommand(trigger);
        }

        static void printUsage(ICommand[] commands) {
            var pad = CLIProgram.GetCommandListPad(commands);
            var commandList = CLIProgram.GetFormattedCommandList(commands);

            commandList.Add("[-v]".PadRight(pad) + " - verbose output");
            commandList.Add("[-s]".PadRight(pad) + " - silent (minimal output)");

            const string header = @"
     gg
    dP8,
   dP Yb
  ,8  `8,
  I8   Yb
  `8b, `8,    ,ggg,    ,ggg,,ggg,    ,ggg,,ggg,   gg     gg
   `'Y88888  i8' '8i  ,8' '8P' '8,  ,8' '8P' '8,  I8     8I
       'Y8   I8, ,8I  I8   8I   8I  I8   8I   8I  I8,   ,8I
        ,88, `YbadP' ,dP   8I   Yb,,dP   8I   Yb,,d8b, ,d8I
    ,ad88888888P'Y8888P'   8I   `Y88P'   8I   `Y8P''Y88P'888
  ,dP''   Yb                                           ,d8I'
 ,8'      I8                                         ,dP'8I
,8'       I8                                        ,8'  8I
I8,      ,8'    A lovely .NET Code Generator        I8   8I
`Y8,___,d8'                                         `8, ,8I
  'Y888P'                                            `Y8P'
";

            const string footer = "Jenny automatically detects *.properties and *.userproperties.\n" +
                                  "If you have multiple property files in one folder, please specify the desired file(s) along with the command.\n\n" +
                                  "EXAMPLE\n" +
                                  "  jenny new Preferences.properties\n" +
                                  "  jenny auto-import\n" +
                                  "  jenny doctor\n" +
                                  "  jenny edit\n" +
                                  "  jenny fix\n" +
                                  "  jenny gen\n" +
                                  "  jenny gen MyOther.properties\n" +
                                  "  jenny gen MyOther.properties username.userproperties";

            Console.WriteLine(header);
            Console.WriteLine("usage:\n" + string.Join("\n", commandList));
            Console.WriteLine("\n" + footer);
        }
    }
}
