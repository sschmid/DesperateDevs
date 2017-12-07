using DesperateDevs.Utils;
using NSpec;

class describe_ObjectCache : nspec {

    void when_caching() {

        ObjectCache cache = null;

        before = () => {
            cache = new ObjectCache();
        };

        it["creates new object pool when requested"] = () => {
            cache.GetObjectPool<TestClass>().should_not_be_null();
        };

        it["returns same object pool already created"] = () => {
            cache.GetObjectPool<TestClass>().should_be_same(cache.GetObjectPool<TestClass>());
        };

        it["returns new instance"] = () => {
            var obj = cache.Get<TestClass>();
            obj.should_not_be_null();
        };

        it["returns pooled instance"] = () => {
            var obj = cache.Get<TestClass>();
            cache.Push(obj);
            cache.Get<TestClass>().should_be_same(obj);
        };

        it["returns custom pushed instance"] = () => {
            var obj = new TestClass();
            cache.Push(obj);
            cache.Get<TestClass>().should_be_same(obj);
        };

        it["registers custom object pool"] = () => {
            var objectPool = new ObjectPool<TestClassWithField>(
                () => new TestClassWithField { value = "myValue" },
                c => c.value = null
            );

            cache.RegisterCustomObjectPool(objectPool);

            cache.GetObjectPool<TestClassWithField>().should_be_same(objectPool);

            var obj = cache.Get<TestClassWithField>();
            obj.value.should_be("myValue");

            cache.Push(obj);
            obj.value.should_be_null();
        };

        it["resets"] = () => {
            var obj = cache.Get<TestClass>();
            cache.Push(obj);
            cache.Reset();
            cache.Get<TestClass>().should_not_be_same(obj);
        };
    }
}
