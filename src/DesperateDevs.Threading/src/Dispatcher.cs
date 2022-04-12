using System;
using System.Collections.Generic;
using System.Threading;

namespace DesperateDevs.Threading
{
    public class Dispatcher
    {
        public bool IsOnThread => _threadId == Thread.CurrentThread.ManagedThreadId;

        readonly int _threadId;
        readonly List<Action> _actions;
        readonly object _lock = new object();

        public Dispatcher(Thread thread)
        {
            _threadId = thread.ManagedThreadId;
            _actions = new List<Action>();
        }

        public void Queue(Action action)
        {
            if (IsOnThread)
            {
                action();
            }
            else
            {
                lock (_lock)
                {
                    _actions.Add(action);
                }
            }
        }

        public void Run()
        {
            if (IsOnThread)
            {
                Action[] actions = null;
                lock (_lock)
                {
                    if (_actions.Count > 0)
                    {
                        actions = _actions.ToArray();
                        _actions.Clear();
                    }
                }

                if (actions != null)
                    foreach (var action in actions)
                        action();
            }
        }
    }
}
