using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TressMontage.Client.Core.Features.Base;
using TressMontage.Client.Features.Base;
using Xamarin.Forms;
using Microsoft.Practices.Unity;
using TressMontage.Client.Core.Features.Home;

namespace TressMontage.Client.Features.Home
{
    public partial class HomeView : ViewBase
    {
        public HomeView()
        {
            InitializeComponent();
        }

        protected override BindableViewModelBase OnPrepareViewModel()
        {
            return App.Container.Resolve<HomeViewModel>();
        }
    }
}
