using Microsoft.Practices.Unity;
using TressMontage.Client.Controls;
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
        private const string DeleteActionSheetHeader = "delete files";
        private const string DeleteActionSheetCancel = "cancel";
        private const string DeleteActionSheetDelete = "delete";

        public DataMagazineView()
        {
            InitializeComponent();

            var deleteMagazinesButton = new ToolbarItem("Delete", null, DeleteMagazines);
            ToolbarItems.Add(deleteMagazinesButton);
        }

        protected override DataMagazineViewModel OnPrepareViewModel()
        {
            return App.Container.Resolve<DataMagazineViewModel>(
                new ParameterOverride("loadingManager", LoadingManager));
        }

        private void DeleteMagazines()
        {
            ViewModel.TryDeleteAsyncCommand.Execute(null);
        }
    }
}
