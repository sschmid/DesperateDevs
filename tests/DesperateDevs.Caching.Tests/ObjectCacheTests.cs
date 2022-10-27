using System.Linq;
using FluentAssertions;
using Xunit;

namespace DesperateDevs.Caching.Tests
{
    public class ObjectCacheTests
    {
        readonly ObjectCache _cache = new ObjectCache();

        [Fact]
        public void CreatesNewObjectPoolWhenRequested()
        {
            _cache.GetObjectPool<object>().Should().NotBeNull();
        }

        [Fact]
        public void ReturnsSameObjectPoolAlreadyCreated()
        {
            _cache.GetObjectPool<object>().Should().BeSameAs(_cache.GetObjectPool<object>());
        }

        [Fact]
        public void ReturnsNewInstance()
        {
            _cache.Get<object>().Should().NotBeNull();
        }

        [Fact]
        public void ReturnsPooledInstance()
        {
            var obj = _cache.Get<object>();
            _cache.Push(obj);
            _cache.Get<object>().Should().BeSameAs(obj);
        }

        [Fact]
        public void ReturnsCustomPushedInstance()
        {
            var obj = new object();
            _cache.Push(obj);
            _cache.Get<object>().Should().BeSameAs(obj);
        }

        [Fact]
        public void RegistersCustomObjectPool()
        {
            var objectPool = new ObjectPool<TestClassWithField>(
                () => new TestClassWithField {Value = "test"},
                c => c.Value = null
            );

            _cache.RegisterCustomObjectPool(objectPool);

            _cache.GetObjectPool<TestClassWithField>().Should().BeSameAs(objectPool);

            var obj = _cache.Get<TestClassWithField>();
            obj.Value.Should().Be("test");

            _cache.Push(obj);
            obj.Value.Should().BeNull();
        }

        [Fact]
        public void Clears()
        {
            var obj = _cache.Get<object>();
            _cache.Push(obj);
            _cache.Clear();
            _cache.Get<object>().Should().NotBeSameAs(obj);
        }

        [Fact]
        public void GetsAllObjectPools()
        {
            var objectPool = new ObjectPool<TestClassWithField>(
                () => new TestClassWithField {Value = "test"},
                c => c.Value = null
            );

            _cache.RegisterCustomObjectPool(objectPool);

            _cache.Get<object>();
            _cache.Get<TestClassWithField>();

            var objectPools = _cache.ObjectPools.ToArray();
            objectPools.Should().HaveCount(2);
            objectPools.Should().Contain(objectPool);
        }
    }
}
