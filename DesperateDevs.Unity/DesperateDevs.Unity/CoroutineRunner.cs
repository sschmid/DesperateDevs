using System;
using System.Collections;
using UnityEngine;

namespace DesperateDevs.Unity {

    public class CoroutineRunner : MonoBehaviour {

        static CoroutineRunner _coroutineRunner;

        static CoroutineRunner runner {
            get {
                if (_coroutineRunner == null) {
                    _coroutineRunner = new GameObject("Coroutine Runner").AddComponent<CoroutineRunner>();
                    DontDestroyOnLoad(_coroutineRunner);
                }

                return _coroutineRunner;
            }
        }

        public static Coroutine Run<T>(IEnumerator enumerator, Action<T> onComplete = null) {
            return runner.StartCoroutine(enumerator, onComplete);
        }

        public static void CancelCoroutine(Coroutine coroutine) {
            runner.StopCoroutine(coroutine);
        }

        public Coroutine StartCoroutine<T>(IEnumerator enumerator, Action<T> onComplete = null) {
            return StartCoroutine(new CoroutineWithData<T>().Wrap(enumerator, onComplete));
        }

        void OnDestroy() {
            _coroutineRunner = null;
        }
    }
}
