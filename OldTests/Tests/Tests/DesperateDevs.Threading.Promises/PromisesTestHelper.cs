using System;
using System.Threading;
using DesperateDevs.Threading.Promises;

public static class PromisesTestHelper {

    public static Promise<T> PromiseWithResult<T>(T result, int delay = 0) {
        return Promise.WithAction(() => {
            if (delay > 0) {
                Thread.Sleep(delay);
            }
            return result;
        });
    }

    public static Promise<T> PromiseWithError<T>(string errorMessage, int delay = 0) {
        return Promise.WithAction<T>(() => {
            if (delay > 0) {
                Thread.Sleep(delay);
            }
            throw new Exception(errorMessage);
        });
    }
}
