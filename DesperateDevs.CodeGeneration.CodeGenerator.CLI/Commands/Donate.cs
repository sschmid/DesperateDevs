using DesperateDevs.CLI.Utils;
using DesperateDevs.Logging;

namespace DesperateDevs.CodeGeneration.CodeGenerator.CLI {

    public class Donate : AbstractCommand {

        public override string trigger { get { return "donate"; } }
        public override string description { get { return null; } }
        public override string example { get { return null; } }

        protected override void run() {

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

            fabl.GetLogger(typeof(Donate).Name).Error(heart);

            System.Diagnostics.Process.Start("https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=BTMLSDQULZ852");
        }
    }
}
