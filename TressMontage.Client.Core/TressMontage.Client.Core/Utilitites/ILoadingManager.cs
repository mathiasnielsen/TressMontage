using System;
namespace TressMontage.Client.Core.Utilities
{
    /// <summary>
    /// A manager which controls the life cycle of a loading indicator.
    /// </summary>
    public interface ILoadingManager
    {
        /// <summary>
        /// Loading event that the view can subscribe to in order to show a spinner. 
        /// </summary>
        event EventHandler Loading;

        /// <summary>
        /// Completed event, which the view will hook into to be able to e.g. dismiss a spinner. 
        /// </summary>
        event EventHandler Completed;

        /// <summary>
        /// Gets a value indicating whether the loading indicator is currently active.
        /// </summary>
        bool IsLoadingCompleted { get; }

        /// <summary>
        /// Begins loading, raising the <see cref="Loading"/> event.
        /// </summary>
        void BeginLoad();

        /// <summary>
        /// Ends loading, raising the <see cref="Completed"/> event.
        /// </summary>
        void EndLoad();

        /// <summary>
        /// Creates a disposable LoadingScope, which internally
        /// calls "BeginLoad" when is constructed, and "EndLoad" when it is disposed. 
        /// This is a convenience method to allow control of the loading cycle through a using block.
        /// </summary>
        LoadingScope CreateLoadingScope();
    }
}
