using DesperateDevs.Logging;
using DesperateDevs.Logging.Formatters;
using NSpec;
using Shouldly;

class describe_DefaultLogMessageFormatter : nspec {

    void when_created() {

        it["formats string"] = () => {
            var f = new DefaultLogMessageFormatter();
            var logger = new Logger("MyLogger");
            var message = f.FormatMessage(logger, LogLevel.Debug, "hi");
            message.ShouldBe("[DEBUG] MyLogger: hi");
        };

        it["pads logLevel"] = () => {
            var f = new DefaultLogMessageFormatter();
            var logger = new Logger("MyLogger");
            var message1 = f.FormatMessage(logger, LogLevel.Debug, "hi");
            var message2 = f.FormatMessage(logger, LogLevel.Warn, "hi");
            message1.ShouldBe("[DEBUG] MyLogger: hi");
            message2.ShouldBe("[WARN]  MyLogger: hi");
        };
    }
}
