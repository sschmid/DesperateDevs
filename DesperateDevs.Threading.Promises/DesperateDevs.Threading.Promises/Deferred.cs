using System;
using System.Threading;

namespace DesperateDevs.Threading.Promises {

    public class Deferred<T> : Promise<T> {

        public Promise<T> promise { get { return this; } }

        public Func<T> action;

        public virtual Promise<T> RunAsync() {
            ThreadPool.QueueUserWorkItem(state => {

                var actionResult = default(T);
                var fulfilled = false;

                try {
                    actionResult = action();
                    fulfilled = true;
                } catch (Exception ex) {
                    Fail(ex);
                }

                if (fulfilled) {
                    Fulfill(actionResult);
                }
            });

            return this;
        }

        public void Fulfill(T result) {
            transitionToFulfilled(result);
        }

        public void Fail(Exception ex) {
            transitionToFailed(ex);
        }

        public void Progress(float progress) {
            setProgress(progress);
        }
    }
}
