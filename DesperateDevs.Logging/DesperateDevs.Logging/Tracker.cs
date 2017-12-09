using System;
using System.Linq;
using System.Net;

namespace DesperateDevs.Logging {

    public class Tracker {

        readonly string _url;
        readonly bool _throwExceptions;

        public Tracker(string host, string endPoint, bool throwExceptions) {
            _url = host + "/" + endPoint;
            _throwExceptions = throwExceptions;
        }

        public virtual void Track(TrackingData data) {
            if (_throwExceptions) {
                WebRequest.Create(buildTrackingCall(data)).GetResponse();
            } else {
                try {
                    WebRequest.Create(buildTrackingCall(data)).GetResponse();
                } catch (Exception) {
                    // ignored
                }
            }
        }

        public virtual void TrackAsync(TrackingData data) {
            if (_throwExceptions) {
                var request = WebRequest.Create(buildTrackingCall(data));
                request.BeginGetResponse(onResponse, request);
            } else {
                try {
                    var request = WebRequest.Create(buildTrackingCall(data));
                    request.BeginGetResponse(onResponse, request);
                } catch (Exception) {
                    // ignored
                }
            }
        }

        void onResponse(IAsyncResult ar) {
            if (_throwExceptions) {
                var request = (WebRequest)ar.AsyncState;
                request.EndGetResponse(ar);
            } else {
                try {
                    var request = (WebRequest)ar.AsyncState;
                    request.EndGetResponse(ar);
                } catch (Exception) {
                    // ignored
                }
            }
        }

        protected string buildTrackingCall(TrackingData data) {
            return data.data.Count != 0
                ? _url + "?" + string.Join("&", data.data
                      .Select(kv => kv.Key + "=" + kv.Value)
                      .ToArray())
                : _url;
        }
    }
}
