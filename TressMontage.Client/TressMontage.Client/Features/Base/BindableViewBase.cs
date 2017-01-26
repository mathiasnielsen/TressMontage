using System;
using TressMontage.Client.Core.Features.Base;
using TressMontage.Client.Services;
using Microsoft.Practices.Unity;

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

                ViewModel.ViewInitialized(this.GetNavigationArgs());
                hasAppeared = true;
            }
            else
            {
                ViewModel.ViewReloading();
            }
        }

        protected virtual TViewModel OnPrepareViewModel()
        { 
            return App.Container.Resolve<TViewModel>();
        }
    }
}
