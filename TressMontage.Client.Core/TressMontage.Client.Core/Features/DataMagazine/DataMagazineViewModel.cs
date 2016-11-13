using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TressMontage.Client.Core.Common;
using TressMontage.Client.Core.Features.Base;
using TressMontage.Client.Core.Http.Clients;
using TressMontage.Client.Core.Services;
using TressMontage.Entities;

namespace TressMontage.Client.Core.Features.DataMagazine
{
    public class DataMagazineViewModel : BindableViewModelBase
    {
        private const string RootFolderName = "DataMagazines";

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
            UpdateCommand = new RelayCommand(Update);
        }

        public RelayCommand<Folder> GoToFolderCommand { get; }

        public RelayCommand UpdateCommand { get; }

        public List<Folder> Folders
        {
            get { return folders; }
            set { Set(ref folders, value); }
        }

        public override async Task OnViewInitialized()
        {
            var rootFolders = await fileInfoManager.GetFoldersAsync(RootFolderName);
            var rootFiles = await fileInfoManager.GetFilesDirectoriesInFolderAsync(RootFolderName);

            await base.OnViewInitialized();
        }

        private async Task SaveFileSync(byte[] file, string path)
        {
            await fileInfoManager.SaveFileAsync(file, path);
        }

        private void GoToFolder(Folder obj)
        {
            navigationService.NavigateToHome();
        }

        private async void Update()
        {
            await RetrieveDataMagazinesFromApiAsync();
        }

        private async Task RetrieveDataMagazinesFromApiAsync()
        {
            var fileDirectories = await api.GetFileNamesAsync();
            var fileDirectory = fileDirectories.FirstOrDefault();
            var file = await api.GetFileAsync(fileDirectory);

            await SaveFileSync(file, fileDirectory);
        }
    }
}
