using FluentAssertions;
using Xunit;

namespace DesperateDevs.Analytics.Tests
{
    public class TestTracker : Tracker
    {
        public string Call { get; private set; }
        public TestTracker() : base("host", "endPoint.php", true) { }
        public override void Track(TrackingData data) => Call = buildTrackingCall(data);
    }

    public class TrackerTests
    {
        readonly TestTracker _tracker = new();
        readonly TrackingData _trackingData = new();

        [Fact]
        public void CreatesTrackingCallWithoutArgs()
        {
            _tracker.Track(_trackingData);
            _tracker.Call.Should().Be("host/endPoint.php");
        }

        [Fact]
        public void CreatesTrackingCallWithArg()
        {
            _trackingData.Add("key",
                "value");
            _tracker.Track(_trackingData);
            _tracker.Call.Should().Be("host/endPoint.php?key=value");
        }

        [Fact]
        public void EscapesArg()
        {
            _trackingData.Add("key", "value1 value2");
            _tracker.Track(_trackingData);
            _tracker.Call.Should().Be("host/endPoint.php?key=value1%20value2");
        }

        [Fact]
        public void CreatesTrackingCallWithMultipleArgs()
        {
            _trackingData.Add("key1", "value1");
            _trackingData.Add("key2", "value2");
            _tracker.Track(_trackingData);
            _tracker.Call.Should().Be("host/endPoint.php?key1=value1&key2=value2");
        }
    }
}
