using System;
using System.Collections;
using UnityEngine;

namespace DesperateDevs.Unity {

    public class CoroutineRunner : MonoBehaviour {

        static CoroutineRunner _coroutineRunner;

        public static void Run<T>(IEnumerator enumerator, Action<T> onComplete = null) {
            if (_coroutineRunner == null) {
                _coroutineRunner = new GameObject("Coroutine Runner").AddComponent<CoroutineRunner>();
                DontDestroyOnLoad(_coroutineRunner);
            }

            _coroutineRunner.StartCoroutine(enumerator, onComplete);
        }

        public void StartCoroutine<T>(IEnumerator enumerator, Action<T> onComplete = null) {
            StartCoroutine(new CoroutineWithData<T>().Wrap(enumerator, onComplete));
        }

        void OnDestroy() {
            _coroutineRunner = null;
        }
    }
}
