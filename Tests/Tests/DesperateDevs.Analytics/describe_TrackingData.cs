using DesperateDevs.Analytics;
using NSpec;
using Shouldly;

class describe_TrackingData : nspec {

    void when_trackingData() {

        TrackingData data = null;

        before = () => {
            data = new TrackingData();
            data.Add("key", "value");
        };

        it["creates empty tracking data"] = () => {
            data = new TrackingData();
            data.Count.ShouldBe(0);
        };

        it["contains added object"] = () => {
            data["key"].ShouldBe("value");
        };
    }
}
