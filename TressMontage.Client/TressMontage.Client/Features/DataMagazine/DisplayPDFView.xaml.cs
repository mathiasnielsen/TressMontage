using TressMontage.Client.Core;
using TressMontage.Client.Features.Base;

namespace TressMontage.Client.Features.DataMagazine
{
    public abstract class DisplayPDFViewBase : BindableViewBase<DisplayPDFViewModel>
    {
    }

    public partial class DisplayPDFView : DisplayPDFViewBase
    {
        public DisplayPDFView()
        {
            InitializeComponent();
        }
    }
}
