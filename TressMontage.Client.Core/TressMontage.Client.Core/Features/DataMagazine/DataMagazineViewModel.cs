using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TressMontage.Client.Core.Common;
using TressMontage.Client.Core.Features.Base;
using TressMontage.Client.Core.Http.Clients;
using TressMontage.Client.Core.Services;
using TressMontage.Entities;

namespace TressMontage.Client.Core.Features.DataMagazine
{
    public class DataMagazineViewModel : BindableViewModelBase
    {
        public const string FolderParameterKey = nameof(FolderParameterKey);
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

        public override async Task OnViewInitialized(Dictionary<string, string> parms)
        {
            Folder folder = null;
            if (parms.Keys.Contains(FolderParameterKey))
            {
                folder = JsonConvert.DeserializeObject<Folder>(parms[FolderParameterKey]);
            }

            if (folder == null)
            {
                var rootFolders = await fileInfoManager.GetFoldersAsync(RootFolderName) as IEnumerable<FileInfo>;
                var rootFiles = await fileInfoManager.GetFilesDirectoriesInFolderAsync(RootFolderName) as IEnumerable<FileInfo>;
                AssignFileInfo(rootFolders, rootFiles);
            }
            else
            {
                var folders = await fileInfoManager.GetFoldersFromFullPath(folder.Path) as IEnumerable<FileInfo>;
                var files = await fileInfoManager.GetFilesDirectoriesInFolderFromFullPathAsync(folder.Path) as IEnumerable<FileInfo>;
                AssignFileInfo(folders, files);
            }

            await base.OnViewInitialized(parms);
        }

        private void AssignFileInfo(IEnumerable<FileInfo> folders, IEnumerable<FileInfo> files)
        {
            var fileInfos = folders.Concat(files);
            FileInfos = fileInfos.ToList();
        }

        private async Task SaveFileSync(byte[] file, string path)
        {
            await fileInfoManager.SaveFileAsync(file, RootFolderName, path);
        }

        private void HandleSelectedFileInfo(FileInfo fileInfo)
        {
            if (fileInfo is Folder)
            {
                var folder = fileInfo as Folder;
                navigationService.NavigateToDataMagazine(folder);
            }

            if (fileInfo.FileType == FileType.pdf)
            {
                navigationService.NavigateToPdfViewer(fileInfo.Path);
            }
        }

        private async void Update()
        {
            await RetrieveDataMagazinesFromApiAsync();
        }

        private async Task RetrieveDataMagazinesFromApiAsync()
        {
            var fileDirectories = await api.GetFileNamesAsync();
            foreach (var fileDirectory in fileDirectories)
            {
                var file = await api.GetFileAsync(fileDirectory);
                await SaveFileSync(file, fileDirectory);
            }
        }
    }
}
