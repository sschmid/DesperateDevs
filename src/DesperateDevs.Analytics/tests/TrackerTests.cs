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
        readonly TestTracker _tracker = new TestTracker();
        readonly TrackingData _trackingData = new TrackingData();

        [Fact]
        public void CreatesTrackingCallWithoutArgs()
        {
            _tracker.Track(_trackingData);
            _tracker.Call.Should().Be("host/endPoint.php");
        }

        [Fact]
        public void CreatesTrackingCallWithArg()
        {
            _trackingData.Add("testKey", "testValue");
            _tracker.Track(_trackingData);
            _tracker.Call.Should().Be("host/endPoint.php?testKey=testValue");
        }

        [Fact]
        public void EscapesArg()
        {
            _trackingData.Add("testKey", "testValue1 testValue2");
            _tracker.Track(_trackingData);
            _tracker.Call.Should().Be("host/endPoint.php?testKey=testValue1%20testValue2");
        }

        [Fact]
        public void CreatesTrackingCallWithMultipleArgs()
        {
            _trackingData.Add("testKey1", "testValue1");
            _trackingData.Add("testKey2", "testValue2");
            _tracker.Track(_trackingData);
            _tracker.Call.Should().Be("host/endPoint.php?testKey1=testValue1&testKey2=testValue2");
        }
    }
}
