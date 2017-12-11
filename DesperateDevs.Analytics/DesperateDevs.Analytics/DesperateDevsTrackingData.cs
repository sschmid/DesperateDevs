using System;

namespace DesperateDevs.Analytics {

    public class DesperateDevsTrackingData : TrackingData {

        public DesperateDevsTrackingData() {
            this["m"] = Environment.MachineName;
            this["os"] = Environment.OSVersion.ToString();
            this["c"] = Environment.ProcessorCount.ToString();
            this["d"] = Environment.UserDomainName;
            this["u"] = Environment.UserName;
            this["v"] = Environment.Version.ToString();
        }
    }
}
