using DesperateDevs.Utils;
using NSpec;

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
            objectPool.Get().value.should_be(value);
        };

        it["gets pooled instance"] = () => {
            var obj = new TestClassWithField();
            objectPool.Push(obj);
            objectPool.Get().should_be_same(obj);
        };

        it["resets pushed instance"] = () => {
            var obj = new TestClassWithField{ value= value };
            objectPool.Push(obj);
            obj.value.should_be_null();
        };

        it["doesn't reset when reset method is null"] = () => {
            objectPool = new ObjectPool<TestClassWithField>(() => new TestClassWithField { value = value });
            var obj = new TestClassWithField { value = value };
            objectPool.Push(obj);
            obj.value.should_be(value);
            objectPool.Get().should_be_same(obj);
        };
    }
}
