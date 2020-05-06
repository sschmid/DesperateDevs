using DesperateDevs.Utils;
using NSpec;
using Shouldly;

class describe_ObjectPool : nspec {

    void when_pooling() {

        const string value = "myValue";

        ObjectPool<TestClassWithField> objectPool = null;

        before = () => {
            objectPool = new ObjectPool<TestClassWithField>(
                () => new TestClassWithField { value = value },
                c => { c.value = null; }
            );
        };

        it["gets new instance from pool"] = () => {
            objectPool.Get().value.ShouldBe(value);
        };

        it["gets pooled instance"] = () => {
            var obj = new TestClassWithField();
            objectPool.Push(obj);
            objectPool.Get().ShouldBeSameAs(obj);
        };

        it["resets pushed instance"] = () => {
            var obj = new TestClassWithField{ value= value };
            objectPool.Push(obj);
            obj.value.ShouldBeNull();
        };

        it["doesn't reset when reset method is null"] = () => {
            objectPool = new ObjectPool<TestClassWithField>(() => new TestClassWithField { value = value });
            var obj = new TestClassWithField { value = value };
            objectPool.Push(obj);
            obj.value.ShouldBe(value);
            objectPool.Get().ShouldBeSameAs(obj);
        };
    }
}
