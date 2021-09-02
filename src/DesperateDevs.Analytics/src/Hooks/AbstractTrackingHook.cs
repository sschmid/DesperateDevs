namespace DesperateDevs.Analytics {

    public abstract class AbstractTrackingHook : ITrackingHook {

        protected virtual string host { get { return "http://desperatedevs.com"; } }
        protected virtual string endPoint { get { return "a/" + name + ".php"; } }
        protected virtual bool throwExceptions { get { return false; } }

        protected abstract string name { get; }

        protected Tracker _tracker;

        public void Track() {
            if (_tracker == null) {
                _tracker = new Tracker(host, endPoint, throwExceptions);
            }
            _tracker.Track(GetData());
        }

        protected abstract TrackingData GetData();
    }
}
