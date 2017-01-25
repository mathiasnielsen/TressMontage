using Microsoft.Practices.Unity;
using TressMontage.Client.Core.Features.Base;
using TressMontage.Client.Core.Features.Home;
using TressMontage.Client.Features.Base;

namespace TressMontage.Client.Features.Home
{
    public abstract class HomeViewBase : BindableViewBase<HomeViewModel> 
    {
    }

    public partial class HomeView : HomeViewBase
    {
        public HomeView()
        {
            InitializeComponent();
        }

        protected override HomeViewModel OnPrepareViewModel()
        {
            return App.Container.Resolve<HomeViewModel>();
        }
    }
}
