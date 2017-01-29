using Microsoft.Practices.Unity;
using TressMontage.Client.Core.Features.Base;
using TressMontage.Client.Features.Base;
using Xamarin.Forms;

namespace TressMontage.Client.Features.DataMagazine
{
    public abstract class DataMagazineViewBase : BindableViewBase<DataMagazineViewModel>
    { 
    }

    public partial class DataMagazineView : DataMagazineViewBase
    {
        public DataMagazineView()
        {
            InitializeComponent();

            var deleteMagazinesButton = new ToolbarItem("Delete", null, DeleteMagazines);
            ToolbarItems.Add(deleteMagazinesButton);
        }

        protected override DataMagazineViewModel OnPrepareViewModel()
        {
            return App.Container.Resolve<DataMagazineViewModel>(new ParameterOverride("loadingManager", LoadingManager));
        }

        private void DeleteMagazines()
        {
            ViewModel.DeleteAllCommand.Execute(null);
        }
    }
}
