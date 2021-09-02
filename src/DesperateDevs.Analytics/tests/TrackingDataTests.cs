using FluentAssertions;
using Xunit;

namespace DesperateDevs.Analytics.Tests
{
    public class TrackingDataTests
    {
        readonly TrackingData _trackingData = new();

        [Fact]
        public void CreatesEmptyTrackingData()
        {
            _trackingData.Count.Should().Be(0);
        }

        [Fact]
        public void AddsKeyAndValue()
        {
            _trackingData.Add("key", "value");
            _trackingData["key"].Should().Be("value");
        }
    }
}
