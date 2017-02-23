using System;
using TressMontage.Client.Controls.Factories;
using TressMontage.Client.Core.Utilities;
using Xamarin.Forms;

namespace TressMontage.Client.Features.Base
{
    public abstract class ViewBase : ContentPage
    {
        private LoadingViewFactory loadingViewFactory;
        private StackLayout loadingContainer;

        public ViewBase()
        {
            BackgroundColor = (Color)Application.Current.Resources["ContentBackgroundColor"];
            LoadingManager = CreateLoadingManager();
            loadingViewFactory = new LoadingViewFactory();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (IsContentRelativeLayout())
            {
                loadingContainer = loadingViewFactory.ConstructView(Content);
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
            loadingContainer.IsVisible = true;
        }

        private void LoadingManager_Completed(object sender, EventArgs e)
        {
            loadingContainer.IsVisible = false;
        }
    }
}
