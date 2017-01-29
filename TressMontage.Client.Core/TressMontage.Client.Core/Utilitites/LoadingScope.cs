using System;
using TressMontage.Client.Core.Extensions;

namespace TressMontage.Client.Core.Utilities
{
    public class LoadingScope : IDisposable
    {
        private readonly ILoadingManager _loadingManager;

        public LoadingScope(ILoadingManager loadingManager)
        {
            loadingManager.ThrowIfParameterIsNull(nameof(loadingManager));

            _loadingManager = loadingManager;
            _loadingManager.BeginLoad();
        }

        public void Dispose()
        {
            _loadingManager.EndLoad();
        }
    }
}
