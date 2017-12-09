using DesperateDevs.Logging;
using NSpec;

class describe_TrackingData : nspec {

    void when_trackingData() {

        TrackingData data = null;

        before = () => {
            data = new TrackingData();
            data.Add("key", "value");
        };

        it["contains default tracking data"] = () => {
            data.data.Count.should_not_be(0);
        };

        it["creates empty tracking data"] = () => {
            data = new TrackingData(false);
            data.data.Count.should_be(0);
        };

        it["contains added object"] = () => {
            data.Get("key").should_be("value");
        };

        it["throws when adding a key that already exists"] = expect<TrackingDataException>(() => {
            data.Add("key", "value");
        });
    }
}
