using System;

namespace DesperateDevs.Analytics {

    public class UserTrackingData : TrackingData {

        public UserTrackingData() {
            this["u"] = Environment.UserName + "@" + Environment.MachineName;
            this["d"] = Environment.ProcessorCount + "@" + Environment.OSVersion;
        }
    }
}
