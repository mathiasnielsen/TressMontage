using System;
using TressMontage.Client.Core.Utilities;
using Xamarin.Forms;
using TressMontage.Client.Controls;
using TressMontage.Client.Features.Common;
using Rg.Plugins.Popup.Extensions;

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
                loadingContainer = new StackLayout();
                loadingContainer.BackgroundColor = new Color(0, 0, 0, 0.4f);

                var loadingLabel = new Label()
                {
                    Text = "Loading..."
                };

                loadingLabel.VerticalOptions = LayoutOptions.Center;
                loadingLabel.HorizontalOptions = LayoutOptions.Center;

                loadingContainer.Children.Add(loadingLabel);

                var centerX = Constraint.RelativeToParent(parent => 0);
                var centerY = Constraint.RelativeToParent(parent => 0);
                var width = Constraint.RelativeToParent(parent => parent.Width);
                var height = Constraint.RelativeToParent(parent => parent.Height);

                var contentAsRelativeLayout = Content as RelativeLayout;
                contentAsRelativeLayout.Children.Add(loadingContainer, centerX, centerY, width, height);

                loadingContainer.IsVisible = false;
            }
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
