using DesperateDevs.Logging;
using NSpec;

class describe_Tracker : nspec {

    void when_Tracking() {

        TestTracker tracker = null;
        TrackingData trackingData = null;

        before = () => {
            tracker = new TestTracker("host", "endPoint.php");
            trackingData = new TrackingData(false);

        };

        it["creates tracking call without args"] = () => {
            tracker.Track(trackingData);
            tracker.call.should_be("host/endPoint.php");
        };

        it["creates tracking call with 1 arg"] = () => {
            trackingData.Add("key", "value");
            tracker.Track(trackingData);
            tracker.call.should_be("host/endPoint.php?key=value");
        };

        it["creates tracking call with multiple args"] = () => {
            trackingData.Add("key", "value");
            trackingData.Add("key2", "value2");
            tracker.Track(trackingData);
            tracker.call.should_be("host/endPoint.php?key=value&key2=value2");
        };
    }
}

class TestTracker : Tracker {

    public string call;

    public TestTracker(string host, string endPoint) : base(host, endPoint, true) {
    }

    public override void Track(TrackingData data) {
        call = buildTrackingCall(data);
    }
}
