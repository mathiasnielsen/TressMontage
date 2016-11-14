using Microsoft.Practices.Unity;
using TressMontage.Client.Core.Features.Base;
using TressMontage.Client.Core.Features.DataMagazine;
using TressMontage.Client.Features.Base;

namespace TressMontage.Client.Features.DataMagazine
{
    public partial class DisplayPdfView : ViewBase
    {
        public DisplayPdfView()
        {
            InitializeComponent();
        }

        protected override BindableViewModelBase OnPrepareViewModel()
        {
            return App.Container.Resolve<DisplayPdfViewModel>();
        }
    }
}
