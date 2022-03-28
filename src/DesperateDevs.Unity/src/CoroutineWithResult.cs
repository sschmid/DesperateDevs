using System;
using System.Collections;

namespace DesperateDevs.Unity
{
    public class CoroutineWithResult<T>
    {
        public T Result;

        public IEnumerator Wrap(IEnumerator enumerator, Action<T> onComplete)
        {
            while (enumerator.MoveNext())
                yield return enumerator.Current;

            Result = (T)enumerator.Current;
            onComplete?.Invoke(Result);
        }
    }
}
