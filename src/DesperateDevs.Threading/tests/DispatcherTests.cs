﻿using System.Threading;
using DesperateDevs.Tests;
using FluentAssertions;
using Xunit;

namespace DesperateDevs.Threading.Tests
{
    public class DispatcherTests
    {
        readonly Dispatcher _dispatcher;
        public DispatcherTests() => _dispatcher = new Dispatcher(Thread.CurrentThread);

        [Fact]
        public void ImmediatelyRunsActionWhenOnThread()
        {
            var didExecute = 0;
            _dispatcher.Queue(() => didExecute += 1);
            didExecute.Should().Be(1);
        }

        [Fact]
        public void WillNotRunImmediatelyOnDifferentThread()
        {
            var didExecute = 0;
            ThreadPool.QueueUserWorkItem(state => { _dispatcher.Queue(() => didExecute += 1); });
            TestHelper.Wait();
            didExecute.Should().Be(0);
        }

        [Fact]
        public void WillRunQueuedActionOnThread()
        {
            var didExecute = 0;
            ThreadPool.QueueUserWorkItem(state => { _dispatcher.Queue(() => didExecute += 1); });
            TestHelper.Wait();
            _dispatcher.Run();
            didExecute.Should().Be(1);
        }

        [Fact]
        public void WillRunAllQueuedActionsOnThread()
        {
            var didExecute = 0;
            ThreadPool.QueueUserWorkItem(state =>
            {
                _dispatcher.Queue(() => didExecute += 1);
                _dispatcher.Queue(() => didExecute += 1);
            });
            TestHelper.Wait();
            _dispatcher.Run();
            didExecute.Should().Be(2);
        }

        [Fact]
        public void WillNotRunQueuedActionWhenNotOnThread()
        {
            var didExecute = 0;
            ThreadPool.QueueUserWorkItem(state => { _dispatcher.Queue(() => didExecute += 1); });
            TestHelper.Wait();
            ThreadPool.QueueUserWorkItem(state =>
            {
                _dispatcher.Run();
                didExecute.Should().Be(0);
            });
            TestHelper.Wait();
        }
    }
}
