using System;
using System.Collections.Generic;
using System.Threading;

namespace DesperateDevs.Threading {

    public class Dispatcher {

        public bool isOnThread { get { return _threadId == Thread.CurrentThread.ManagedThreadId; } }

        readonly int _threadId;
        readonly List<Action> _actions;
        readonly object _lock = new object();

        public Dispatcher(Thread thread) {
            _threadId = thread.ManagedThreadId;
            lock (_lock) {
                _actions = new List<Action>();
            }
        }

        public void Queue(Action action) {
            if (isOnThread) {
                action();
            } else {
                lock (_lock) {
                    _actions.Add(action);
                }
            }
        }

        public void Run() {
            if (isOnThread) {
                Action[] actions = null;
                lock (_lock) {
                    if (_actions.Count > 0) {
                        actions = _actions.ToArray();
                        _actions.Clear();
                    }
                }

                if (actions != null) {
                    foreach (var action in actions) {
                        action();
                    }
                }
            }
        }
    }
}
