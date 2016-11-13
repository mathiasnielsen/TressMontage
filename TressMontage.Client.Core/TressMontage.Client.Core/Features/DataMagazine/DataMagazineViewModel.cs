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

        private List<FileInfo> fileInfos;

        public DataMagazineViewModel(ITressMontageApi api, INavigationService navigationService, IFileInfoManager fileInfoManager)
        {
            this.navigationService = navigationService;
            this.fileInfoManager = fileInfoManager;
            this.api = api;

            FileInfoSelectedCommand = new RelayCommand<FileInfo>(HandleSelectedFileInfo);
            UpdateCommand = new RelayCommand(Update);
        }

        public RelayCommand<FileInfo> FileInfoSelectedCommand { get; }

        public RelayCommand UpdateCommand { get; }

        public List<FileInfo> FileInfos
        {
            get { return fileInfos; }
            set { Set(ref fileInfos, value); }
        }

        public override async Task OnViewInitialized()
        {
            var rootFolders = await fileInfoManager.GetFoldersAsync(RootFolderName) as IEnumerable<FileInfo>;
            var rootFiles = await fileInfoManager.GetFilesDirectoriesInFolderAsync(RootFolderName) as IEnumerable<FileInfo>;

            var fileInfos = rootFolders.Concat(rootFiles);
            FileInfos = fileInfos.ToList();

            await base.OnViewInitialized();
        }

        private async Task SaveFileSync(byte[] file, string path)
        {
            await fileInfoManager.SaveFileAsync(file, path);
        }

        private void HandleSelectedFileInfo(FileInfo fileInfo)
        {
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
