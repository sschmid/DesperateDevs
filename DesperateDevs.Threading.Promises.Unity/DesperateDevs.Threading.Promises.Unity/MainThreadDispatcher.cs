using System;
using System.Threading;
using UnityEngine;

namespace DesperateDevs.Threading.Promises.Unity {

    public class MainThreadDispatcher : MonoBehaviour {

        public static bool isOnMainThread { get { return _dispatcher.isOnThread; } }

        static Dispatcher _dispatcher;
        static bool _isInitialized;

        public static void Initialize() {
            if (!_isInitialized) {
                _isInitialized = true;
                _dispatcher = new Dispatcher(Thread.CurrentThread);
                var go = new GameObject(typeof(MainThreadDispatcher).Name);
                go.AddComponent<MainThreadDispatcher>();
                DontDestroyOnLoad(go);
            }
        }

        public static void Queue(Action action) {
            _dispatcher.Queue(action);
        }

        void Update() {
            _dispatcher.Run();
        }
    }
}
