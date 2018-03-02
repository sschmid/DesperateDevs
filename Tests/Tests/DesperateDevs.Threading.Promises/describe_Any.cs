﻿using NSpec;
using System.Collections.Generic;
using DesperateDevs.Threading.Promises;

class describe_Any : nspec {

    const int delay = 5;

    void when_running_in_parallel_with_any() {

        Promise<string> p1 = null;
        Promise<string> p2 = null;
        Promise<string> p3 = null;
        Promise<string> p4 = null;
        Promise<string> promise = null;

        context["when at least one promise fulfills"] = () => {

            List<float> eventProgresses = null;

            before = () => {
                p1 = PromisesTestHelper.PromiseWithError<string>("error 42", delay);
                p2 = PromisesTestHelper.PromiseWithResult("42", delay * 2);
                p3 = PromisesTestHelper.PromiseWithResult("43", delay * 3);
                p4 = PromisesTestHelper.PromiseWithResult("44", delay * 3);
                eventProgresses = new List<float>();
                promise = Promise.Any(p1, p2, p3, p4);
                promise.OnProgressed += eventProgresses.Add;
                promise.Await();
            };

            it["is fulfilled"] = () => promise.state.should_be(PromiseState.Fulfilled);
            it["first promise to be complete is fulfilled"] = () => p2.state.should_be(PromiseState.Fulfilled);
            it["has result of first complete promise"] = () => promise.result.should_be("42");

            context["progress"] = () => {

                it["calculates progress of promise with highest progress"] = () => {
                    eventProgresses.Count.should_be(1);
                    eventProgresses[0].should_be(1f);
                };

                it["has initial max progress"] = () => {
                    var deferred = new Deferred<object>();
                    deferred.Progress(0.5f);
                    var p = PromisesTestHelper.PromiseWithResult<object>("42", delay * 2);
                    var any = Promise.Any(deferred, p);
                    any.progress.should_be(0.5f);
                    deferred.Fulfill(null);
                    any.Await();
                };
            };
        };

        context["when all promises fail"] = () => {

            before = () => {
                p1 = PromisesTestHelper.PromiseWithError<string>("error 42", delay);
                p2 = PromisesTestHelper.PromiseWithError<string>("error 43", delay * 2);
                p3 = PromisesTestHelper.PromiseWithError<string>("error 44", delay * 3);
                promise = Promise.Any(p1, p2, p3);
                promise.Await();
            };

            it["fails"] = () => promise.state.should_be(PromiseState.Failed);
            it["has no result"] = () => promise.result.should_be_null();
            it["has error"] = () => promise.error.should_not_be_null();
        };
    }
}
