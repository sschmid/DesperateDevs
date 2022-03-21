using DesperateDevs.CLI.Utils;
using DesperateDevs.Logging;

namespace DesperateDevs.CodeGeneration.CodeGenerator.CLI
{
    public class DonateCommand : AbstractCommand
    {
        public override string trigger => "donate";
        public override string description => null;
        public override string group => null;
        public override string example => null;

        protected override void run()
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
