using System.Threading;
using DesperateDevs.Threading;
using NSpec;

class describe_Dispatcher : nspec {

    void when_dispatching() {

        Dispatcher dispatcher = null;

        before = () => { dispatcher = new Dispatcher(Thread.CurrentThread); };

        it["immediately runs action when on thread"] = () => {
            var didExecute = 0;
            dispatcher.Queue(() => didExecute += 1);
            didExecute.should_be(1);
        };

        it["won't run on different thread"] = () => {
            var didExecute = 0;
            ThreadPool.QueueUserWorkItem(state => { dispatcher.Queue(() => didExecute += 1); });
            this.Wait();
            didExecute.should_be(0);
        };

        it["will run queued action on thread"] = () => {
            var didExecute = 0;
            ThreadPool.QueueUserWorkItem(state => { dispatcher.Queue(() => didExecute += 1); });
            this.Wait();
            dispatcher.Run();
            didExecute.should_be(1);
        };

        it["will run all queued actions on thread"] = () => {
            var didExecute = 0;
            ThreadPool.QueueUserWorkItem(state => {
                dispatcher.Queue(() => didExecute += 1);
                dispatcher.Queue(() => didExecute += 1);
            });
            this.Wait();
            dispatcher.Run();
            didExecute.should_be(2);
        };
    }
}
