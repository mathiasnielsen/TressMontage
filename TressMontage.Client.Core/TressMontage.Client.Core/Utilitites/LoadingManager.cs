using System;
namespace TressMontage.Client.Core.Utilities
{
public class LoadingManager : ILoadingManager
    {
        public LoadingManager()
        {
            IsLoadingCompleted = false;
        }

        /// <summary>
        /// Loading event, which the view will hook into,
        /// to be able to e.g. show a spinner. 
        /// </summary>
        public event EventHandler Loading;

        /// <summary>
        /// Completed event, which the view will hook into
        /// to be able to e.g. dismiss a spinner. 
        /// </summary>
        public event EventHandler Completed;

        public bool IsLoadingCompleted { get; private set; }

        public bool UseProgress { get; private set; }

        /// <summary>
        /// Invokes loading.
        /// </summary>
        public void BeginLoad()
        {
            IsLoadingCompleted = false;
            OnLoading();
        }

        /// <summary>
        /// Dismisses loading.
        /// </summary>
        public void EndLoad()
        {
            IsLoadingCompleted = true;
            OnCompleted();
        }

        /// <summary>
        /// Creates a disposable LoadingScope, which internally
        /// calls "BeginLoad" when it is constrcuted, and "EndLoad"
        /// when it is disposed. 
        /// </summary>
        public LoadingScope CreateLoadingScope(bool useProgress = false, Action completion = null)
        {
            UseProgress = useProgress;
            return new LoadingScope(this, completion);
        }

        private void OnLoading()
        {
            Loading?.Invoke(this, null);
        }

        private void OnCompleted()
        {
            Completed?.Invoke(this, null);
        }
    }
}
