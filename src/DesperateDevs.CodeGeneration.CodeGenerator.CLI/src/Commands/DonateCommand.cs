using DesperateDevs.Cli.Utils;
using DesperateDevs.Logging;

namespace DesperateDevs.CodeGeneration.CodeGenerator.CLI
{
    public class DonateCommand : AbstractCommand
    {
        public override string Trigger => "donate";
        public override string Description => null;
        public override string Group => null;
        public override string Example => null;

        protected override void Run()
        {
            const string heart = @"
        @@@@@@@@           @@@@@@@@
      @@@@@@@@@@@@       @@@@@@@@@@@@
    @@@@@@@@@@@@@@@@   @@@@@@@@@@@@@@@@
  @@@@@@@@@@@@@@@@@@@ @@@@@@@@@@@@@@@@@@@
 @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
 @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
  @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
    @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
      @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        @@@@@@@@@@@@@@@@@@@@@@@@@@@
          @@@@@@@@@@@@@@@@@@@@@@@
            @@@@@@@@@@@@@@@@@@@
              @@@@@@@@@@@@@@@
                @@@@@@@@@@@
                  @@@@@@@
                   @@@@
                    @@
";

            Sherlog.GetLogger(typeof(DonateCommand)).Error(heart);

            System.Diagnostics.Process.Start("https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=BTMLSDQULZ852");
        }
    }
}
