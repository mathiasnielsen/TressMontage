using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TressMontage.Client.Core.Features.Base;
using TressMontage.Client.Core.Features.DataMagazine;
using TressMontage.Client.Features.Base;
using Xamarin.Forms;
using Microsoft.Practices.Unity;
using System.IO;

namespace TressMontage.Client.Features.DataMagazine
{
    public partial class DataMagazineView : ViewBase
    {
        public DataMagazineView()
        {
            InitializeComponent();
        }

        protected override BindableViewModelBase OnPrepareViewModel()
        {
            return App.Container.Resolve<DataMagazineViewModel>();
        }
    }
}
