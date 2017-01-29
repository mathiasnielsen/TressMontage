using System;
using TressMontage.Client.Core.Utilities;
using Xamarin.Forms;

namespace TressMontage.Client.Features.Base
{
    public abstract class ViewBase : ContentPage
    {
        private ActivityIndicator _activityIndicator;

        public ViewBase()
        {
            BackgroundColor = (Color)Application.Current.Resources["ContentBackgroundColor"];
            LoadingManager = CreateLoadingManager();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _activityIndicator = new ActivityIndicator();
        }

        protected ILoadingManager LoadingManager { get; }

        private ILoadingManager CreateLoadingManager()
        {
            var loadingManager = new LoadingManager();

            loadingManager.Loading += LoadingManager_Loading;
            loadingManager.Completed += LoadingManager_Completed;

            return loadingManager;
        }

        private void LoadingManager_Loading(object sender, EventArgs e)
        {
            Content.IsVisible = false;
        }

        private void LoadingManager_Completed(object sender, EventArgs e)
        {
            Content.IsVisible = true;
        }
    }
}
