using FluentAssertions;
using Xunit;

namespace DesperateDevs.Caching.Tests
{
    public class ObjectPoolTests
    {
        const string Value = "test";

        ObjectPool<TestClassWithField> _objectPool;

        public ObjectPoolTests()
        {
            _objectPool = new ObjectPool<TestClassWithField>(
                () => new TestClassWithField {Value = Value},
                o => { o.Value = null; }
            );
        }

        [Fact]
        public void GetsNewInstanceFromPool()
        {
            _objectPool.Get().Value.Should().Be(Value);
        }

        [Fact]
        public void GetsPooledInstance()
        {
            var obj = new TestClassWithField();
            _objectPool.Push(obj);
            _objectPool.Get().Should().BeSameAs(obj);
        }

        [Fact]
        public void ResetsPushedInstance()
        {
            var obj = new TestClassWithField {Value = Value};
            _objectPool.Push(obj);
            obj.Value.Should().BeNull();
        }

        [Fact]
        public void DoesNotResetWhenResetMethodIsNull()
        {
            _objectPool = new ObjectPool<TestClassWithField>(() => new TestClassWithField {Value = Value});
            var obj = new TestClassWithField {Value = Value};
            _objectPool.Push(obj);
            obj.Value.Should().Be(Value);
        }

        [Fact]
        public void DrainsPool()
        {
            var obj1 = new TestClassWithField();
            var obj2 = new TestClassWithField();
            _objectPool.Push(obj1);
            _objectPool.Push(obj2);

            var objects = _objectPool.Drain();
            objects.Should().HaveCount(2);
            objects.Should().Contain(obj1);
            objects.Should().Contain(obj2);

            var obj = _objectPool.Get();
            obj.Should().NotBeSameAs(obj1);
            obj.Should().NotBeSameAs(obj2);
        }

        [Fact]
        public void ClearsPool()
        {
            var obj1 = new TestClassWithField();
            var obj2 = new TestClassWithField();
            _objectPool.Push(obj1);
            _objectPool.Push(obj2);

            _objectPool.Clear();

            var obj = _objectPool.Get();
            obj.Should().NotBeSameAs(obj1);
            obj.Should().NotBeSameAs(obj2);
        }
    }

    public class TestClassWithField
    {
        public string Value;
    }
}
