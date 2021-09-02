using System.Threading;
using DesperateDevs.Threading;
using NSpec;
using Shouldly;

class describe_Dispatcher : nspec {

    void when_dispatching() {

        Dispatcher dispatcher = null;

        before = () => { dispatcher = new Dispatcher(Thread.CurrentThread); };

        it["immediately runs action when on thread"] = () => {
            var didExecute = 0;
            dispatcher.Queue(() => didExecute += 1);
            didExecute.ShouldBe(1);
        };

        it["won't run immediately on different thread"] = () => {
            var didExecute = 0;
            ThreadPool.QueueUserWorkItem(state => { dispatcher.Queue(() => didExecute += 1); });
            this.Wait();
            didExecute.ShouldBe(0);
        };

        it["will run queued action on thread"] = () => {
            var didExecute = 0;
            ThreadPool.QueueUserWorkItem(state => { dispatcher.Queue(() => didExecute += 1); });
            this.Wait();
            dispatcher.Run();
            didExecute.ShouldBe(1);
        };

        it["will run all queued actions on thread"] = () => {
            var didExecute = 0;
            ThreadPool.QueueUserWorkItem(state => {
                dispatcher.Queue(() => didExecute += 1);
                dispatcher.Queue(() => didExecute += 1);
            });
            this.Wait();
            dispatcher.Run();
            didExecute.ShouldBe(2);
        };

        it["won't run queued action when not on thread"] = () => {
            var didExecute = 0;
            ThreadPool.QueueUserWorkItem(state => { dispatcher.Queue(() => didExecute += 1); });
            this.Wait();
            ThreadPool.QueueUserWorkItem(state => {
                dispatcher.Run();
                didExecute.ShouldBe(0);
            });
            this.Wait();
        };
    }
}
