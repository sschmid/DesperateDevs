using DesperateDevs.Analytics;

namespace DesperateDevs.CodeGeneration.Plugins {

    public abstract class TrackingPostProcessor : IPostProcessor {

        public abstract string name { get; }
        public abstract int priority { get; }
        public abstract bool runInDryMode { get; }

        public CodeGenFile[] PostProcess(CodeGenFile[] files) {
            new Tracker(getHost(), getEndPoint(), false)
                .Track(GetData(files));

            return files;
        }

        protected virtual string getHost() {
            return "http://desperatedevs.com";
        }

        protected virtual string getEndPoint() {
            return "a/" + GetName() + ".php";
        }

        protected abstract string GetName();
        protected abstract TrackingData GetData(CodeGenFile[] files);
    }
}
