using System;
using TressMontage.Client.Controls.Factories;
using TressMontage.Client.Core.Utilities;
using Xamarin.Forms;

namespace TressMontage.Client.Features.Base
{
    public abstract class ViewBase : ContentPage
    {
        public ViewBase()
        {
            BackgroundColor = (Color)Application.Current.Resources["ContentBackgroundColor"];
            LoadingManager = CreateLoadingManager();
        }

        protected LoadingView LoadingView { get; private set; }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (IsContentRelativeLayout())
            {
                LoadingView = new LoadingView(Content);
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        private bool IsContentRelativeLayout()
        {
            var result = Content is RelativeLayout;

            if (result == false)
            {
                System.Diagnostics.Debug.WriteLine("Not relativeLayout");
            }

            return result;
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
            LoadingView.ProgressLabel.IsVisible = LoadingManager.UseProgress;
            LoadingView.IsVisible = true;
        }

        private void LoadingManager_Completed(object sender, EventArgs e)
        {
            LoadingView.IsVisible = false;
        }
    }
}
