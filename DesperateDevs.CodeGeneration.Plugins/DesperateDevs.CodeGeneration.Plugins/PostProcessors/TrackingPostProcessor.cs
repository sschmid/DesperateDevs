using System.Linq;
using DesperateDevs.Logging;

namespace DesperateDevs.CodeGeneration.Plugins {

    public abstract class TrackingPostProcessor : IPostProcessor {

        public string name { get { return null; } }
        public int priority { get { return 9999; } }
        public bool runInDryMode { get { return false; } }

        public CodeGenFile[] PostProcess(CodeGenFile[] files) {
            var fileCount = files.Length;
            var componentCount = files.Count(f => f.fileName.Contains("Component.cs"));
            var contextCount = files.Count(f => f.fileName.Contains("Context.cs"));

            var data = GetData();
            data.Add("fs", fileCount.ToString());
            data.Add("cs", fileCount.ToString());
            data.Add("cx", fileCount.ToString());

            new Tracker(GetHost(), GetEntPoint(), false).Track(data);
            return files;
        }

        protected abstract string GetHost();
        protected abstract string GetEntPoint();
        protected abstract TrackingData GetData();
    }
}
