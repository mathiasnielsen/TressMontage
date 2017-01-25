using System;
using TressMontage.Client.Core.Features.Base;
using TressMontage.Client.Services;

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

        protected abstract TViewModel OnPrepareViewModel();
    }
}
