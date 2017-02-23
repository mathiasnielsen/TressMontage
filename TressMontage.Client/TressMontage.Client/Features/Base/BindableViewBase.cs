using System;
using TressMontage.Client.Core.Features.Base;
using TressMontage.Client.Services;
using Microsoft.Practices.Unity;
using Xamarin.Forms;

namespace TressMontage.Client.Features.Base
{
    public abstract class BindableViewBase<TViewModel>: ViewBase
        where TViewModel : BindableViewModelBase
    {
        private bool hasAppeared;

        public TViewModel ViewModel { get; set; }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (hasAppeared == false)
            {
                ViewModel = OnPrepareViewModel();
                BindingContext = ViewModel;

                InitializeViewBaseBindings();
                ViewModel.ViewInitialized(this.GetNavigationArgs());
                hasAppeared = true;
            }
            else
            {
                ViewModel.ViewReloading();
            }
        }

        private void InitializeViewBaseBindings()
        {
            if (LoadingView != null)
            {
                var converter = new ProgressToPercentageTextConverter();
                LoadingView.ProgressLabel.SetBinding(Label.TextProperty, "LoadingProgress", converter: converter);
                LoadingView.ProgressLabel.BindingContext = BindingContext;
            }
        }

        protected virtual TViewModel OnPrepareViewModel()
        { 
            return App.Container.Resolve<TViewModel>();
        }
    }
}
