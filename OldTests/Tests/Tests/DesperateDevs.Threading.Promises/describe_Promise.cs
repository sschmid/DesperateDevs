using NSpec;
using System;
using DesperateDevs.Threading.Promises;
using Shouldly;

class describe_Promise : nspec {

    const int delay = 5;

    void when_running_an_expensive_action() {

        Promise<string> promise = null;
        string eventResult = null;
        Exception eventError = null;
        float eventProgress = 0f;
        bool fulfilledCalled = false, failedCalled = false, progressCalled = false;

        before = () => {
            eventResult = null;
            eventError = null;
            eventProgress = -1f;
            fulfilledCalled = failedCalled = progressCalled = false;
        };

        after = () => promise.Await();

        context["before action finished"] = () => {

            before = () => {
                promise = PromisesTestHelper.PromiseWithResult("42", delay);
                promise.OnFulfilled += result => {
                    eventResult = result;
                    fulfilledCalled = true;
                };
                promise.OnFailed += error => {
                    eventError = error;
                    failedCalled = true;
                };
                promise.OnProgressed += progress => {
                    eventProgress = progress;
                    progressCalled = true;
                };
            };

            it["is unfulfilled"] = () => promise.state.ShouldBe(PromiseState.Unfulfilled);
            it["has progressed 0%"] = () => promise.progress.ShouldBe(0f);
            it["has no result"] = () => promise.result.ShouldBeNull();
            it["has no error"] = () => promise.error.ShouldBeNull();

            context["await"] = () => {

                it["blocks until finished"] = () => {
                    promise.Await();
                    promise.state.ShouldBe(PromiseState.Fulfilled);
                };

                it["does nothing when promise already finished"] = () => {
                    promise.Await();
                    promise.Await();
                    true.ShouldBeTrue();
                };

                it["returns promise"] = () => promise.Await().result.ShouldBe("42");
            };

            context["events"] = () => {

                it["doesn't call OnFulfilled"] = () => fulfilledCalled.ShouldBeFalse();
                it["doesn't call OnFail"] = () => failedCalled.ShouldBeFalse();
                it["doesn't call OnProgress"] = () => progressCalled.ShouldBeFalse();
            };

            context["when action finished"] = () => {

                before = () => promise.Await();

                it["is fulfilled"] = () => promise.state.ShouldBe(PromiseState.Fulfilled);
                it["has progressed 100%"] = () => promise.progress.ShouldBe(1f);
                it["has result"] = () => promise.result.ShouldBe("42");
                it["has no error"] = () => promise.error.ShouldBeNull();

                context["events"] = () => {

                    it["calls OnFulfilled"] = () => eventResult.ShouldBe("42");
                    it["calls OnFulfilled when adding callback"] = () => {
                        string lateResult = null;
                        promise.OnFulfilled += result => lateResult = result;
                        lateResult.ShouldBe("42");
                    };
                    it["doesn't call OnFailed"] = () => failedCalled.ShouldBeFalse();
                    it["doesn't call OnFailed when adding callback"] = () => {
                        var called = false;
                        promise.OnFailed += error => called = true;
                        called.ShouldBeFalse();
                    };
                    it["calls OnProgress"] = () => eventProgress.ShouldBe(1f);
                };
            };
        };

        context["when action throws an exception"] = () => {

            before = () => {
                promise = PromisesTestHelper.PromiseWithError<string>("error 42", delay);
                promise.OnFulfilled += result => {
                    eventResult = result;
                    fulfilledCalled = true;
                };
                promise.OnFailed += error => {
                    eventError = error;
                    failedCalled = true;
                };
                promise.OnProgressed += progress => {
                    eventProgress = progress;
                    eventProgress = progress;
                };
                promise.Await();
            };

            it["failed"] = () => promise.state.ShouldBe(PromiseState.Failed);
            it["has progressed 0%"] = () => promise.progress.ShouldBe(0f);
            it["has no result"] = () => promise.result.ShouldBeNull();
            it["has error"] = () => promise.error.Message.ShouldBe("error 42");

            context["events"] = () => {

                it["doesn't call OnFulfilled"] = () => fulfilledCalled.ShouldBeFalse();
                it["doesn't call OnFulfilled when adding callback"] = () => {
                    var called = false;
                    promise.OnFulfilled += result => called = true;
                    called.ShouldBeFalse();
                };
                it["calls OnFailed"] = () => eventError.Message.ShouldBe("error 42");
                it["calls OnFailed when adding callback"] = () => {
                    Exception lateError = null;
                    promise.OnFailed += error => lateError = error;
                    lateError.Message.ShouldBe("error 42");
                };
                it["doesn't call OnProgress"] = () => progressCalled.ShouldBeFalse();
            };
        };

        context["when creating a custom promise with Deferred"] = () => {

            Deferred<string> deferred = null;
            int progressEventCalled = 0;

            before = () => {
                deferred = new Deferred<string>();
                progressEventCalled = 0;
                deferred.OnProgressed += progress => {
                    eventProgress = progress;
                    progressEventCalled++;
                };
            };

            it["progresses"] = () => {
                deferred.Progress(0.3f);
                deferred.Progress(0.6f);
                deferred.Progress(0.9f);
                deferred.Progress(1f);

                eventProgress.ShouldBe(1f);
                deferred.promise.progress.ShouldBe(1f);
                progressEventCalled.ShouldBe(4);
            };

            it["doesn't call OnProgressed when setting equal progress"] = () => {
                deferred.Progress(0.3f);
                deferred.Progress(0.3f);
                deferred.Progress(0.3f);
                deferred.Progress(0.6f);

                eventProgress.ShouldBe(0.6f);
                deferred.promise.progress.ShouldBe(0.6f);
                progressEventCalled.ShouldBe(2);
            };

            it["doesn't call OnProgressed when adding callback when progress is less than 100%"] = () => {
                deferred.Progress(0.3f);
                var called = false;
                deferred.OnProgressed += progress => called = true;
                called.ShouldBeFalse();
            };

            it["calls OnProgressed when adding callback when progress is 100%"] = () => {
                deferred.Progress(1f);
                var called = false;
                deferred.OnProgressed += progress => called = true;
                called.ShouldBeTrue();
            };
        };

        context["when event handler throws exceptions"] = () => {

            xit["OnFulfilled"] = expect<ArgumentOutOfRangeException>(() => {
                promise = PromisesTestHelper.PromiseWithResult("42", delay);

                promise.OnFulfilled += result => {
                    throw new ArgumentOutOfRangeException();
                };
                promise.Await();
            });

            xit["OnFailed"] = expect<ArgumentOutOfRangeException>(() => {
                promise = PromisesTestHelper.PromiseWithError<string>("Fail Test", delay);

                promise.OnFailed += error => {
                    throw new ArgumentOutOfRangeException();
                };
                promise.Await();
            });
        };

        context["wrap"] = () => {

            Promise<object> wrapped = null;

            context["when wrapped promise fulfills"] = () => {

                it["forwards fulfill"] = () => {
                    promise = PromisesTestHelper.PromiseWithResult("42", delay);
                    wrapped = promise.Wrap<object>();
                    wrapped.Await();
                    wrapped.state.ShouldBe(PromiseState.Fulfilled);
                };

                it["forwards fail"] = () => {
                    promise = PromisesTestHelper.PromiseWithError<string>("error 42", delay);
                    wrapped = promise.Wrap<object>();
                    wrapped.Await();
                    wrapped.state.ShouldBe(PromiseState.Failed);
                };

                it["forwards progress"] = () => {
                    var deferred = new Deferred<string>();
                    wrapped = deferred.Wrap<object>();
                    deferred.Progress(0.5f);
                    wrapped.progress.ShouldBe(0.5f);
                };

                it["has initial progress"] = () => {
                    var deferred = new Deferred<string>();
                    deferred.Progress(0.5f);
                    wrapped = deferred.Wrap<object>();
                    wrapped.progress.ShouldBe(0.5f);
                };
            };
        };

        context["toString"] = () => {

            it["returns description of unfulfilled promise"] = () => {
                var deferred = new Deferred<string>();
                deferred.Progress(0.1234567890f);
                deferred.promise.ToString().ShouldBe("[Promise<String>: state = Unfulfilled, progress = 0.123]");
            };

            it["returns description of fulfilled promise"] = () => {
                promise = PromisesTestHelper.PromiseWithResult("42");
                promise.Await();
                promise.ToString().ShouldBe("[Promise<String>: state = Fulfilled, result = 42]");
            };

            it["returns description of failed promise"] = () => {
                promise = PromisesTestHelper.PromiseWithError<string>("error 42");
                promise.Await();
                promise.ToString().ShouldBe("[Promise<String>: state = Failed, progress = 0, error = error 42]");
            };
        };
    }
}
