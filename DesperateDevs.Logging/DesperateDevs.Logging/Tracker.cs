using System;
using System.Linq;
using System.Net;

namespace DesperateDevs.Logging {

    public class Tracker {

        readonly string _url;
        readonly bool _throwExceptions;
        readonly Logger _logger = fabl.GetLogger(typeof(Tracker).Name);

        public Tracker(string host, string endPoint, bool throwExceptions) {
            _url = host + "/" + endPoint;
            _throwExceptions = throwExceptions;
        }

        public virtual void Track(TrackingData data) {
            if (_throwExceptions) {
                getResponse(data);
            } else {
                try {
                    getResponse(data);
                } catch (Exception) {
                    // ignored
                }
            }
        }

        public virtual void TrackAsync(TrackingData data) {
            if (_throwExceptions) {
                getResponseAsync(data);
            } else {
                try {
                    getResponseAsync(data);
                } catch (Exception) {
                    // ignored
                }
            }
        }

        HttpWebRequest createWebRequest(TrackingData data) {
            var request = (HttpWebRequest)WebRequest.Create(buildTrackingCall(data));
            request.Timeout = 1000;

            return request;
        }

        void getResponse(TrackingData data) {
            createWebRequest(data).GetResponse();
        }

        void getResponseAsync(TrackingData data) {
            var request = createWebRequest(data);
            request.BeginGetResponse(onResponse, request);
        }

        void onResponse(IAsyncResult ar) {
            if (_throwExceptions) {
                endResponse(ar);
            } else {
                try {
                    endResponse(ar);
                } catch (Exception) {
                    // ignored
                }
            }
        }

        void endResponse(IAsyncResult ar) {
            var request = (WebRequest)ar.AsyncState;
            request.EndGetResponse(ar);
        }

        protected string buildTrackingCall(TrackingData data) {
            var call = data.data.Count != 0
                ? _url + Uri.EscapeUriString("?" + string.Join("&", data.data
                      .Select(kv => kv.Key + "=" + kv.Value)
                      .ToArray()))
                : _url;

            _logger.Debug(call);

            return call;
        }
    }
}
