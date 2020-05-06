using NSpec;
using System.Collections.Generic;
using DesperateDevs.Threading.Promises;
using Shouldly;

class describe_Collect : nspec {

    const int delay = 5;

    void when_running_in_parallel_with_collect() {

        Promise<object> p1 = null;
        Promise<object> p2 = null;
        Promise<object[]> promise = null;
        List<float> eventProgresses = null;

        context["when running with a promise that fulfills and one that fails"] = () => {

            before = () => {
                eventProgresses = new List<float>();
                p1 = PromisesTestHelper.PromiseWithError<object>("error 42", delay * 2);
                p2 = PromisesTestHelper.PromiseWithResult<object>("42", delay);
                promise = Promise.Collect(p1, p2);
                promise.OnProgressed += eventProgresses.Add;
                promise.Await();
            };

            it["is fulfilled"] = () => promise.state.ShouldBe(PromiseState.Fulfilled);
            it["has progressed 100%"] = () => promise.progress.ShouldBe(1f);
            it["has result"] = () => promise.result.ShouldNotBeNull();
            it["has no error"] = () => promise.error.ShouldBeNull();
            it["has results at correct index"] = () => {
                (promise.result[0]).ShouldBeNull();
                (promise.result[1]).ShouldBe("42");
            };

            it["calls progress"] = () => {
                eventProgresses.Count.ShouldBe(2);
                eventProgresses[0].ShouldBe(0.5f);
                eventProgresses[1].ShouldBe(1f);
            };

            it["has initial progress"] = () => {
                var deferred = new Deferred<object>();
                deferred.Progress(0.5f);
                p2 = PromisesTestHelper.PromiseWithResult<object>("42", delay);
                var collect = Promise.Collect(deferred, p2);
                collect.progress.ShouldBe(0.25f);
                deferred.Fulfill(null);
                collect.Await();
            };
        };

        context["when all promises fail"] = () => {

            before = () => {
                eventProgresses = new List<float>();
                p1 = PromisesTestHelper.PromiseWithError<object>("error 42", delay * 2);
                p2 = PromisesTestHelper.PromiseWithError<object>("error 43", delay);
                promise = Promise.Collect(p1, p2);
                promise.OnProgressed += eventProgresses.Add;
                promise.Await();
            };

            it["progresses"] = () => {
                eventProgresses.Count.ShouldBe(2);
                eventProgresses[0].ShouldBe(0.5f);
                eventProgresses[1].ShouldBe(1f);
            };
        };
    }
}
