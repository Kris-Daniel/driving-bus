
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Utils.Extensions {
    
    public static class SemaphoreSlimExtensions
    {

        public static async Task<IDisposable> UseWaitAsync( this System.Threading.SemaphoreSlim semaphore, CancellationToken cancelToken = default)
        {
            await semaphore.WaitAsync(cancelToken).ConfigureAwait(false);
            return new ReleaseWrapper(semaphore);
        }

        class ReleaseWrapper : IDisposable
        {
            readonly System.Threading.SemaphoreSlim _semaphore;

            bool _isDisposed;

            public ReleaseWrapper(System.Threading.SemaphoreSlim semaphore)
            {
                _semaphore = semaphore;
            }

            public void Dispose()
            {
                if (_isDisposed)
                    return;

                if (_semaphore.CurrentCount != 0)
                    Debug.LogError($"[SemaphoreSlim] WTF: count = {_semaphore.CurrentCount}");
                _semaphore.ReleaseIfZero();
                _isDisposed = true;
            }
        }
    }
    
}
