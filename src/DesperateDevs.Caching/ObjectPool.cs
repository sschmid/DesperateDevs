using System;
using System.Collections.Generic;

namespace DesperateDevs.Caching
{
    public class ObjectPool<T>
    {
        readonly Func<T> _factoryMethod;
        readonly Action<T> _resetMethod;
        readonly Stack<T> _objectPool;

        public ObjectPool(Func<T> factoryMethod, Action<T> resetMethod = null)
        {
            _factoryMethod = factoryMethod;
            _resetMethod = resetMethod;
            _objectPool = new Stack<T>();
        }

        public T Get() => _objectPool.Count == 0
            ? _factoryMethod()
            : _objectPool.Pop();

        public void Push(T obj)
        {
            _resetMethod?.Invoke(obj);
            _objectPool.Push(obj);
        }

        public T[] Drain()
        {
            var objects = _objectPool.ToArray();
            _objectPool.Clear();
            return objects;
        }

        public List<T> Drain(List<T> buffer)
        {
            buffer.Clear();
            buffer.AddRange(_objectPool);
            _objectPool.Clear();
            return buffer;
        }

        public void Clear() => _objectPool.Clear();
    }
}
