using System;
using System.Collections;

namespace DesperateDevs.Unity {

    public class CoroutineWithData<T> {

        public T result;

        public IEnumerator Wrap(IEnumerator enumerator, Action<T> onComplete) {
            while (enumerator.MoveNext()) {
                yield return enumerator.Current;
            }

            result = (T)enumerator.Current;
            if (onComplete != null) {
                onComplete(result);
            }
        }
    }
}
