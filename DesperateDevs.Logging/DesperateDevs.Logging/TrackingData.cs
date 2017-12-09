using System;
using System.Collections.Generic;

namespace DesperateDevs.Logging {

    public class TrackingData {

        public Dictionary<string, string> data { get { return _data; } }
        readonly Dictionary<string, string> _data = new Dictionary<string, string>();

        public TrackingData(bool addDefaults = true) {
            if (addDefaults) {
                Add("m", Environment.MachineName);
                Add("os", Environment.OSVersion.ToString());
                Add("c", Environment.ProcessorCount.ToString());
                Add("d", Environment.UserDomainName);
                Add("u", Environment.UserName);
                Add("v", Environment.Version.ToString());
            }
        }

        public void Add(string key, string value) {
            if (_data.ContainsKey(key)) {
                throw new TrackingDataException("Key " + key + " already exists!");
            }
            _data[key] = value;
        }

        public string Get(string key) {
            return _data[key];
        }
    }

    public class TrackingDataException : Exception {

        public TrackingDataException(string message) : base(message) {
        }
    }
}
