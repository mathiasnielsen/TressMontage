using System;
using System.Threading;
using System.Threading.Tasks;

namespace TressMontage.Client.Core.Utilities
{
    /// <summary>
    /// BEWARE: An AsyncLock is not re-entrant, so using the same instance of the lock within it's scope
    /// will cause a "deadlock". Where as a normal "lock" is re-entrant, meaning the same thread can enter it again 
    /// without blocking itself, only acquisition from other threads are blocked.
    /// </summary>
    public sealed class AsyncLock
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private readonly Task<IDisposable> _releaser;

        public AsyncLock()
        {
            _releaser = Task.FromResult((IDisposable)new Releaser(this));
        }

        /// <summary>
        /// LockAsync in not re-entrant.
        /// </summary>
        public Task<IDisposable> LockAsync()
        {
            var wait = _semaphore.WaitAsync();
            return wait.IsCompleted ?
                _releaser :
                wait.ContinueWith((_, state) => (IDisposable)state, _releaser.Result, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        }

        /// <summary>
        /// Lock in not re-entrant.
        /// </summary>
        public IDisposable Lock()
        {
            _semaphore.Wait();
            return _releaser.Result;
        }

        private sealed class Releaser : IDisposable
        {
            private readonly AsyncLock lockToRelease;

            internal Releaser(AsyncLock lockToRelease)
            {
                this.lockToRelease = lockToRelease;
            }

            public void Dispose()
            {
                lockToRelease._semaphore.Release();
            }
        }
    }
}
