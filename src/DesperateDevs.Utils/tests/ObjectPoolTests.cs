using FluentAssertions;
using Xunit;

namespace DesperateDevs.Utils.Tests
{
    public class ObjectPoolTests
    {
        const string Value = "test";

        ObjectPool<TestClassWithField> _objectPool;

        public ObjectPoolTests()
        {
            _objectPool = new ObjectPool<TestClassWithField>(
                () => new TestClassWithField {Value = Value},
                c => { c.Value = null; }
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
            var obj = new TestClassWithField();
            _objectPool.Push(obj);

            var objects = _objectPool.Drain();
            objects.Length.Should().Be(1);
            objects[0].Should().BeSameAs(obj);

            _objectPool.Get().Should().NotBeSameAs(obj);
        }
    }

    public class TestClassWithField
    {
        public string Value;
    }
}
