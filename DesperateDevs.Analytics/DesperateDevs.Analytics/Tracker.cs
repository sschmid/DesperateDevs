using System;
using System.Linq;
using System.Net;
using DesperateDevs.Logging;

namespace DesperateDevs.Analytics {

    public class Tracker {

        readonly string _url;
        readonly bool _throwExceptions;
        readonly Logger _logger = fabl.GetLogger(typeof(Tracker));

        public Tracker(string host, string endPoint, bool throwExceptions) {
            _url = host + "/" + endPoint;
            _throwExceptions = throwExceptions;
        }

        public virtual WebResponse Track(TrackingData data) {
            if (_throwExceptions) {
                return getResponse(data);
            }

            try {
                return getResponse(data);
            } catch (Exception) {
                return null;
            }
        }

        public virtual void TrackAsync(TrackingData data, Action<WebResponse> onComplete) {
            if (_throwExceptions) {
                getResponseAsync(data, onComplete);
            } else {
                try {
                    getResponseAsync(data, onComplete);
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

        WebResponse getResponse(TrackingData data) {
            return createWebRequest(data).GetResponse();
        }

        void getResponseAsync(TrackingData data, Action<WebResponse> onComplete) {
            var request = createWebRequest(data);
            var state = new AsyncRequestState(request, onComplete);
            request.BeginGetResponse(onResponse, state);
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
            var state = (AsyncRequestState)ar.AsyncState;
            var response = state.request.EndGetResponse(ar);
            if (state.onComplete != null) {
                state.onComplete(response);
            }
        }

        protected string buildTrackingCall(TrackingData data) {
            var call = data.Count != 0
                ? _url + Uri.EscapeUriString("?" + string.Join("&", data
                                                 .Select(kv => kv.Key + "=" + kv.Value)
                                                 .ToArray()))
                : _url;

            _logger.Trace(call);

            return call;
        }
    }

    class AsyncRequestState {

        public readonly HttpWebRequest request;
        public readonly Action<WebResponse> onComplete;

        public AsyncRequestState(HttpWebRequest request, Action<WebResponse> onComplete) {
            this.request = request;
            this.onComplete = onComplete;
        }
    }
}
