using DesperateDevs.Utils;
using NSpec;
using Shouldly;

class describe_ObjectCache : nspec {

    void when_caching() {

        ObjectCache cache = null;

        before = () => {
            cache = new ObjectCache();
        };

        it["creates new object pool when requested"] = () => {
            cache.GetObjectPool<TestClass>().ShouldNotBeNull();
        };

        it["returns same object pool already created"] = () => {
            cache.GetObjectPool<TestClass>().ShouldBeSameAs(cache.GetObjectPool<TestClass>());
        };

        it["returns new instance"] = () => {
            var obj = cache.Get<TestClass>();
            obj.ShouldNotBeNull();
        };

        it["returns pooled instance"] = () => {
            var obj = cache.Get<TestClass>();
            cache.Push(obj);
            cache.Get<TestClass>().ShouldBeSameAs(obj);
        };

        it["returns custom pushed instance"] = () => {
            var obj = new TestClass();
            cache.Push(obj);
            cache.Get<TestClass>().ShouldBeSameAs(obj);
        };

        it["registers custom object pool"] = () => {
            var objectPool = new ObjectPool<TestClassWithField>(
                () => new TestClassWithField { value = "myValue" },
                c => c.value = null
            );

            cache.RegisterCustomObjectPool(objectPool);

            cache.GetObjectPool<TestClassWithField>().ShouldBeSameAs(objectPool);

            var obj = cache.Get<TestClassWithField>();
            obj.value.ShouldBe("myValue");

            cache.Push(obj);
            obj.value.ShouldBeNull();
        };

        it["resets"] = () => {
            var obj = cache.Get<TestClass>();
            cache.Push(obj);
            cache.Reset();
            cache.Get<TestClass>().ShouldNotBeSameAs(obj);
        };
    }
}
