﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TressMontage.Client.Core.Common;
using TressMontage.Client.Core.Extensions;
using TressMontage.Client.Core.Features.Base;
using TressMontage.Client.Core.Http.Clients;
using TressMontage.Client.Core.Services;
using TressMontage.Client.Core.Utilities;
using TressMontage.Client.Utilities;
using TressMontage.Entities;
using TressMontage.Utilities;

namespace TressMontage.Client.Features.DataMagazine
{
    public class DataMagazineViewModel : BindableViewModelBase
    {
        public const string RelativeDirectoryParameterKey = nameof(RelativeDirectoryParameterKey);
        private const string RootFolderName = "DataMagazines";

        private readonly IFileInfoManager fileInfoManager;
        private readonly INavigationService navigationService;
        private readonly ITressMontageApi api;
        private readonly FileInfoMapper fileMapper;
        private readonly ILoadingManager loadingManager;

        private List<FileInfo> fileInfos;
        private string title;
        private string subDirectory;

        public DataMagazineViewModel(ITressMontageApi api, INavigationService navigationService, IFileInfoManager fileInfoManager, ILoadingManager loadingManager)
        {
            this.navigationService = navigationService.ThrowIfParameterIsNull(nameof(navigationService));
            this.fileInfoManager = fileInfoManager.ThrowIfParameterIsNull(nameof(fileInfoManager));
            this.api = api.ThrowIfParameterIsNull(nameof(api));
            this.loadingManager = loadingManager.ThrowIfParameterIsNull(nameof(loadingManager));

            fileMapper = new FileInfoMapper();

            FileInfoSelectedCommand = new RelayCommand<FileInfo>(HandleSelectedFileInfo);
            UpdateCommand = new RelayCommand(UpdateAsync);
            DeleteAllCommand = new RelayCommand(DeleteAllAsync);
        }

        public RelayCommand<FileInfo> FileInfoSelectedCommand { get; }

        public RelayCommand UpdateCommand { get; }

        public RelayCommand DeleteAllCommand { get; }

        public List<FileInfo> FileInfos
        {
            get { return fileInfos; }
            set { Set(ref fileInfos, value); }
        }

        public string Title
        {
            get { return title; }
            set { Set(ref title, value); }
        }

        public override async Task OnViewInitialized(Dictionary<string, string> navigationParameters)
        {
            using (loadingManager.CreateLoadingScope())
            {
                await fileInfoManager.RetrieveOrConstructFeatureRootDirectory(RootFolderName);

                if (navigationParameters.ContainsKey(RelativeDirectoryParameterKey))
                {
                    subDirectory = navigationParameters[RelativeDirectoryParameterKey];
                    Title = $"../{Path.GetFileName(subDirectory)}";
                    await GetDataFromDirectoryAsync(subDirectory);
                }
                else
                {
                    Title = "Data Magazines";
                    await GetDataFromRootFolderAsync();
                }   
            }
        }

        public override async Task OnViewReloaded()
        {
            using (loadingManager.CreateLoadingScope())
            {
                var isSubDirectory = string.IsNullOrWhiteSpace(subDirectory) == false;
                if (isSubDirectory)
                {
                    await GetDataFromDirectoryAsync(subDirectory);
                }
                else
                {
                    await GetDataFromRootFolderAsync();
                }   
            }
        }

        private async Task GetDataFromRootFolderAsync()
        {
            var rootFolders = await fileInfoManager.GetFoldersFromRootFoldersAsync(RootFolderName);
            var rootFiles = await fileInfoManager.GetFilesDirectoriesInFolderAsync(RootFolderName);

            var folders = fileMapper.MapFolderToFileInfo(rootFolders);
            var files = fileMapper.MapFilesToFileInfo(rootFiles);

            var filesAndFoldersAsFileInfos = folders.Concat(files);
            FileInfos = filesAndFoldersAsFileInfos.ToList();
        }

        private async Task GetDataFromDirectoryAsync(string relativeDirectory)
        {
            var rootFolders = await fileInfoManager.GetFoldersInDirectoryAsync(RootFolderName, relativeDirectory);
            var rootFiles = await fileInfoManager.GetFilesDirectoriesInFolderAsync(relativeDirectory);

            var folders = fileMapper.MapFolderToFileInfo(rootFolders);
            var files = fileMapper.MapFilesToFileInfo(rootFiles);

            var filesAndFoldersAsFileInfos = folders.Concat(files);
            FileInfos = filesAndFoldersAsFileInfos.ToList();
        }

        private async Task SaveFileSync(byte[] file, string path)
        {
            await fileInfoManager.SaveFileAsync(file, RootFolderName, path);
        }

        private void HandleSelectedFileInfo(FileInfo fileInfo)
        {
            switch (fileInfo.Type)
            {
                case FileTypes.FOLDER:
                    navigationService.NavigateToDataMagazine(fileInfo.Path);
                    break;

                case FileTypes.PDF:
                    navigationService.NavigateToDisplayPDF(fileInfo.Path);
                    break;
            }
        }

        private async void UpdateAsync()
        {
            using (loadingManager.CreateLoadingScope())
            {
                await RetrieveDataMagazinesFromApiAsync();
                await GetDataFromRootFolderAsync();
            }
        }

        private async Task RetrieveDataMagazinesFromApiAsync()
        {
            var fileDirectories = await api.GetFileNamesAsync();
            foreach (var fileDirectory in fileDirectories)
            {
                var file = await api.GetFileAsync(fileDirectory.Path);
                await SaveFileSync(file, fileDirectory.Path);
            }
        }

        private async void DeleteAllAsync()
        {
            using (loadingManager.CreateLoadingScope())
            { 
                var hasDeleted = await fileInfoManager.DeleteMagazineFolderAsync(RootFolderName);
                if (hasDeleted)
                {
                    FileInfos = new List<FileInfo>();
                }
            }
        }
    }
}
