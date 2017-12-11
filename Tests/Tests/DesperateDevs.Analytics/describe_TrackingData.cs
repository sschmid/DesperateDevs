using DesperateDevs.Analytics;
using NSpec;

class describe_TrackingData : nspec {

    void when_trackingData() {

        TrackingData data = null;

        before = () => {
            data = new TrackingData();
            data.Add("key", "value");
        };

        it["creates empty tracking data"] = () => {
            data = new TrackingData();
            data.Count.should_be(0);
        };

        it["contains added object"] = () => {
            data["key"].should_be("value");
        };
    }
}
