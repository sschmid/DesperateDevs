using System;
using System.Linq;
using System.Net;
using DesperateDevs.Logging;

namespace DesperateDevs.Analytics
{
    public class Tracker
    {
        readonly string _url;
        readonly bool _throwExceptions;
        readonly Logger _logger = fabl.GetLogger(typeof(Tracker));

        public Tracker(string host, string endPoint, bool throwExceptions)
        {
            _url = host + "/" + endPoint;
            _throwExceptions = throwExceptions;
        }

        public virtual void Track(TrackingData data)
        {
            if (_throwExceptions)
            {
                getResponse(data);
                return;
            }

            try
            {
                getResponse(data);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public virtual void TrackAsync(TrackingData data, Action onComplete)
        {
            if (_throwExceptions)
            {
                getResponseAsync(data, onComplete);
            }
            else
            {
                try
                {
                    getResponseAsync(data, onComplete);
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }

        HttpWebRequest createWebRequest(TrackingData data)
        {
            var request = (HttpWebRequest)WebRequest.Create(buildTrackingCall(data));
            request.Timeout = 3000;
            return request;
        }

        void getResponse(TrackingData data)
        {
            createWebRequest(data).GetResponse().Close();
        }

        void getResponseAsync(TrackingData data, Action onComplete)
        {
            var request = createWebRequest(data);
            var state = new AsyncRequestState(request, onComplete);
            request.BeginGetResponse(onResponse, state);
        }

        void onResponse(IAsyncResult ar)
        {
            if (_throwExceptions)
            {
                endResponse(ar);
            }
            else
            {
                try
                {
                    endResponse(ar);
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }

        void endResponse(IAsyncResult ar)
        {
            var state = (AsyncRequestState)ar.AsyncState;
            state.request.EndGetResponse(ar).Close();
            if (state.onComplete != null)
            {
                state.onComplete();
            }
        }

        protected string buildTrackingCall(TrackingData data)
        {
            var call = data.Count != 0
                ? _url + Uri.EscapeUriString("?" + string.Join("&", data
                                                 .Select(kv => kv.Key + "=" + kv.Value)
                                                 .ToArray()))
                : _url;

            _logger.Trace(call);

            return call;
        }
    }

    class AsyncRequestState
    {
        public readonly HttpWebRequest request;
        public readonly Action onComplete;

        public AsyncRequestState(HttpWebRequest request, Action onComplete)
        {
            this.request = request;
            this.onComplete = onComplete;
        }
    }
}
