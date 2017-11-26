using System.IO;

namespace DesperateDevs.Logging.CLI {

    public class Watch : AbstractSocketCommand {

        public override string trigger { get { return "watch"; } }
        public override string description { get { return "Watch a file"; } }
        public override string example { get { return "fabl watch [filePath]"; } }

        protected override void run() {
            using (var stream = File.OpenRead(_args[1])) {
                using (var reader = new StreamReader(stream)) {
                    long length = 0;
                    while (true) {
                        while (!reader.EndOfStream) {
                            fabl.Info(reader.ReadLine());
                        }

                        length = stream.Length;

                        while (stream.Length == length) { }

                        if (stream.Length < length) {
                            stream.Position = 0;
                            reader.DiscardBufferedData();
                        }
                    }
                }
            }
        }
    }
}
