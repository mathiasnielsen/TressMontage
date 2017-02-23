using System;
using TressMontage.Client.Core.Extensions;

namespace TressMontage.Client.Core.Utilities
{
    public class LoadingScope : IDisposable
    {
        private readonly ILoadingManager loadingManager;
        private readonly Action completion;

        public LoadingScope(ILoadingManager loadingManager, Action completion)
        {
            loadingManager.ThrowIfParameterIsNull(nameof(loadingManager));

            this.loadingManager = loadingManager;
            this.loadingManager.BeginLoad();
            this.completion = completion;
        }

        public void Dispose()
        {
            completion?.Invoke();
            this.loadingManager.EndLoad();
        }
    }
}
