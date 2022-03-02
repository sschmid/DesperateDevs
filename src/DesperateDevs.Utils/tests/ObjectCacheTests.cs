using FluentAssertions;
using Xunit;

namespace DesperateDevs.Utils.Tests
{
    public class ObjectCacheTests
    {
        readonly ObjectCache _cache = new ObjectCache();

        [Fact]
        public void CreatesNewObjectPoolWhenRequested()
        {
            _cache.GetObjectPool<EmptyTestClass>().Should().NotBeNull();
        }

        [Fact]
        public void ReturnsSameObjectPoolAlreadyCreated()
        {
            _cache.GetObjectPool<EmptyTestClass>().Should().BeSameAs(_cache.GetObjectPool<EmptyTestClass>());
        }

        [Fact]
        public void ReturnsNewInstance()
        {
            _cache.Get<EmptyTestClass>().Should().NotBeNull();
        }

        [Fact]
        public void ReturnsPooledInstance()
        {
            var obj = _cache.Get<EmptyTestClass>();
            _cache.Push(obj);
            _cache.Get<EmptyTestClass>().Should().BeSameAs(obj);
        }

        [Fact]
        public void ReturnsCustomPushedInstance()
        {
            var obj = new EmptyTestClass();
            _cache.Push(obj);
            _cache.Get<EmptyTestClass>().Should().BeSameAs(obj);
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
        public void Resets()
        {
            var obj = _cache.Get<EmptyTestClass>();
            _cache.Push(obj);
            _cache.Reset();
            _cache.Get<EmptyTestClass>().Should().NotBeSameAs(obj);
        }
    }
}
