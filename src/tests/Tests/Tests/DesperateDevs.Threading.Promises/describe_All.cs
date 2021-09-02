using NSpec;
using System.Collections.Generic;
using DesperateDevs.Threading.Promises;
using Shouldly;

class describe_All : nspec {

    const int delay = 5;

    void when_running_in_parallel_with_all() {

        Promise<object> p1 = null;
        Promise<object> p2 = null;
        Promise<object[]> promise = null;
        List<float> eventProgresses = null;

        context["when all promises fulfill"] = () => {

            before = () => {
                eventProgresses = new List<float>();
                p1 = PromisesTestHelper.PromiseWithResult<object>(42, delay);
                p2 = PromisesTestHelper.PromiseWithResult<object>("42", 2 * delay);
                promise = Promise.All(p1, p2);
                promise.OnProgressed += eventProgresses.Add;
                promise.Await();
            };

            it["is fulfilled"] = () => promise.state.ShouldBe(PromiseState.Fulfilled);
            it["has progressed 100%"] = () => promise.progress.ShouldBe(1f);
            it["has result"] = () => promise.result.ShouldNotBeNull();
            it["has no error"] = () => promise.error.ShouldBeNull();
            it["has results at correct index"] = () => {
                (promise.result[0]).ShouldBe(42);
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
                p2 = PromisesTestHelper.PromiseWithResult<object>("42", 2 * delay);
                promise = Promise.All(deferred, p2);
                promise.progress.ShouldBe(0.25f);
                deferred.Fulfill(null);
                promise.Await();
            };
        };

        context["when a promise fails"] = () => {

            before = () => {
                eventProgresses = new List<float>();
                p1 = PromisesTestHelper.PromiseWithResult<object>(42, delay);
                p2 = PromisesTestHelper.PromiseWithError<object>("error 42", 2 * delay);
                promise = Promise.All(p1, p2);
                promise.OnProgressed += eventProgresses.Add;
                promise.Await();
            };

            it["failed"] = () => promise.state.ShouldBe(PromiseState.Failed);
            it["has progressed 50%"] = () => promise.progress.ShouldBe(0.5f);
            it["has no result"] = () => promise.result.ShouldBeNull();
            it["has error"] = () => promise.error.Message.ShouldBe("error 42");
            it["calls progress"] = () => {
                eventProgresses.Count.ShouldBe(1);
                eventProgresses[0].ShouldBe(0.5f);
            };
        };
    }
}
