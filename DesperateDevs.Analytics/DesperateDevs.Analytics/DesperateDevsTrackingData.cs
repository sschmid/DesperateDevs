using System;

namespace DesperateDevs.Analytics {

    public class DesperateDevsTrackingData : TrackingData {

        public DesperateDevsTrackingData() {
            this["u"] = Environment.UserName + "@" + Environment.MachineName;
            this["d"] = Environment.ProcessorCount + "@" + Environment.OSVersion;
        }
    }
}
