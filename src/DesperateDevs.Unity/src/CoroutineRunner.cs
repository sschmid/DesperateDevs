using System;
using System.Collections;
using UnityEngine;

namespace DesperateDevs.Unity
{
    public class CoroutineRunner : MonoBehaviour
    {
        static CoroutineRunner Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameObject(nameof(CoroutineRunner)).AddComponent<CoroutineRunner>();
                    DontDestroyOnLoad(_instance);
                }

                return _instance;
            }
        }

        static CoroutineRunner _instance;

        public static Coroutine Run(IEnumerator enumerator) => Instance.StartCoroutine(enumerator);

        public static Coroutine Run<T>(IEnumerator enumerator, Action<T> onComplete) =>
            Instance.StartCoroutine(new CoroutineWithResult<T>().Wrap(enumerator, onComplete));

        public static void Stop(Coroutine coroutine) => Instance.StopCoroutine(coroutine);

        public static void StopAll() => Instance.StopAllCoroutines();

        void OnDestroy() => _instance = null;
    }
}
