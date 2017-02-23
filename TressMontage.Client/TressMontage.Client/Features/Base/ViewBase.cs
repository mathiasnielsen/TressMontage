using System;
using TressMontage.Client.Core.Utilities;
using Xamarin.Forms;

namespace TressMontage.Client.Features.Base
{
    public abstract class ViewBase : ContentPage
    {
        private StackLayout loadingContainer;

        public ViewBase()
        {
            BackgroundColor = (Color)Application.Current.Resources["ContentBackgroundColor"];
            LoadingManager = CreateLoadingManager();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (IsContentRelativeLayout())
            {
                var stackLayout = new StackLayout();
                stackLayout.Padding = new Thickness(20, 20, 20, 40);
                stackLayout.VerticalOptions = LayoutOptions.CenterAndExpand;
                stackLayout.HorizontalOptions = LayoutOptions.CenterAndExpand;

                var activitySpinner = new ActivityIndicator();
                activitySpinner.WidthRequest = 100;
                activitySpinner.HeightRequest = 100;
                activitySpinner.Color = Color.White;
                activitySpinner.IsVisible = true;
                activitySpinner.IsRunning = true;

                stackLayout.Children.Add(activitySpinner);

                loadingContainer = new StackLayout();
                loadingContainer.VerticalOptions = LayoutOptions.FillAndExpand;
                loadingContainer.HorizontalOptions = LayoutOptions.FillAndExpand;
                loadingContainer.BackgroundColor = new Color(0, 0, 0, 0.4f);

                loadingContainer.Children.Add(stackLayout);

                var centerX = Constraint.RelativeToParent(parent => 0);
                var centerY = Constraint.RelativeToParent(parent => 0);
                var width = Constraint.RelativeToParent(parent => parent.Width);
                var height = Constraint.RelativeToParent(parent => parent.Height);

                var contentAsRelativeLayout = Content as RelativeLayout;
                contentAsRelativeLayout.Children.Add(loadingContainer, centerX, centerY, width, height);

                loadingContainer.IsVisible = false;
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
