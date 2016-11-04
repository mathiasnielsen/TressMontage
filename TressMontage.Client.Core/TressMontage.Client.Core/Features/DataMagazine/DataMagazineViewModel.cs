using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TressMontage.Client.Core.Common;
using TressMontage.Client.Core.Features.Base;
using TressMontage.Client.Core.Http;
using TressMontage.Client.Core.Http.Clients;
using TressMontage.Client.Core.Services;
using TressMontage.Entities;

namespace TressMontage.Client.Core.Features.DataMagazine
{
    public class DataMagazineViewModel : BindableViewModelBase
    {
        private readonly IFileInfoManager fileInfoManager;
        private readonly INavigationService navigationService;
        private readonly ITressMontageApi api;

        private List<Folder> folders;

        public DataMagazineViewModel(ITressMontageApi api, INavigationService navigationService, IFileInfoManager fileInfoManager)
        {
            this.navigationService = navigationService;
            this.fileInfoManager = fileInfoManager;
            this.api = api;

            GoToFolderCommand = new RelayCommand<Folder>(GoToFolder);
        }

        public RelayCommand<Folder> GoToFolderCommand { get; set; }

        public List<Folder> Folders
        {
            get { return folders; }
            set { Set(ref folders, value); }
        }

        public override async Task OnViewInitialized()
        {
            Folders = await fileInfoManager.GetCurrentFolderPathsAsync();

            await base.OnViewInitialized();
        }

        private void GoToFolder(Folder obj)
        {
            navigationService.NavigateToHome();
        }
    }
}
