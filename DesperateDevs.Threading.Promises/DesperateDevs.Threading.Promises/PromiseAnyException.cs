using System;

namespace DesperateDevs.Threading.Promises {

    public class PromiseAnyException : Exception {

        public PromiseAnyException() : base("All promises did fail!") {
        }
    }
}
