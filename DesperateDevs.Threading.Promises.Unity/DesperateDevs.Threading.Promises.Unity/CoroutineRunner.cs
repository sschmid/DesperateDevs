using System;
using System.Collections;
using UnityEngine;

namespace DesperateDevs.Threading.Promises.Unity {

    public class CoroutineRunner : MonoBehaviour {

        static CoroutineRunner _coroutineRunner;

        public static Coroutine<T> StartRoutine<T>(IEnumerator coroutine, Action<Coroutine<T>> onComplete = null) {
            if (_coroutineRunner == null) {
                _coroutineRunner = new GameObject("CoroutineRunner").AddComponent<CoroutineRunner>();
                DontDestroyOnLoad(_coroutineRunner);
            }

            return _coroutineRunner.StartCoroutine(coroutine, onComplete);
        }

        public Coroutine<T> StartCoroutine<T>(IEnumerator coroutine, Action<Coroutine<T>> onComplete = null) {
            var coroutineObject = new Coroutine<T>();
            coroutineObject.coroutine = StartCoroutine(coroutineObject.WrapRoutine(coroutine));
            if (onComplete != null) {
                StartCoroutine(onCompleteCoroutine(coroutineObject, onComplete));
            }

            return coroutineObject;
        }

        IEnumerator onCompleteCoroutine<T>(Coroutine<T> coroutine, Action<Coroutine<T>> onComplete) {
            yield return coroutine.coroutine;
            onComplete(coroutine);
        }

        void OnDestroy() {
            _coroutineRunner = null;
        }
    }
}
