//using System;
//using System.Collections;
//
//namespace DesperateDevs.Threading.Promises.Unity {
//
//    public class UnityDeferred<T> : Deferred<T> {
//
//        public Func<IEnumerator> coroutine;
//
//        public override Promise<T> RunAsync() {
//            MainThreadDispatcher.Dispatch(() => CoroutineRunner.StartRoutine<T>(coroutine(), c => {
//
//                var actionResult = default(T);
//                var fulfilled = false;
//
//                try {
//                    actionResult = c.returnValue;
//                    fulfilled = true;
//                } catch (Exception ex) {
//                    Fail(ex);
//                }
//
//                if (fulfilled) {
//                    Fulfill(actionResult);
//                }
//            }));
//
//            return this;
//        }
//    }
//}
