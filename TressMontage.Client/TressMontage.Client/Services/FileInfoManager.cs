﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PCLStorage;
using TressMontage.Client.Core.Services;

namespace TressMontage.Client.Services
{
    public class FileInfoManager : IFileInfoManager
    {
        private const char DirectorySeperator = '/';
        private readonly IFolder rootFolder;

        public FileInfoManager()
        {
            rootFolder = FileSystem.Current.LocalStorage;
        }

        public async Task<IFolder> RetrieveOrConstructFeatureRootDirectory(string featureDirectoryName)
        {
            IFolder featureFolder;
            var doesFeatureDirectoryExist = await rootFolder.CheckExistsAsync(featureDirectoryName);
            if (doesFeatureDirectoryExist == ExistenceCheckResult.NotFound)
            {
                featureFolder = await rootFolder.CreateFolderAsync(featureDirectoryName, CreationCollisionOption.ReplaceExisting);
            }
            else
            {
                featureFolder = await rootFolder.GetFolderAsync(featureDirectoryName);
            }

            return featureFolder;
        }

        public async Task<IList<IFolder>> GetFoldersFromRootFoldersAsync(string featureDirectoryName)
        {
            await RetrieveOrConstructFeatureRootDirectory(featureDirectoryName);

            var featureRootFolder = await rootFolder.GetFolderAsync(featureDirectoryName);
            var featureFolders = await featureRootFolder.GetFoldersAsync();

            return featureFolders;
        }

        public async Task<IList<IFolder>> GetFoldersInDirectoryAsync(string featureDirectoryName, string relativeFolderPath)
        {
            IList<IFolder> folders;

            var doesFolderExist = await rootFolder.CheckExistsAsync(relativeFolderPath);
            if (doesFolderExist != ExistenceCheckResult.NotFound)
            {
                folders = await GetFolderDirectoriesAsync(relativeFolderPath);
                return folders;
            }

            return default(IList<IFolder>);
        }

        public async Task<IList<IFile>> GetFilesDirectoriesInFolderAsync(string relativeFolderPath)
        {
            var folder = await rootFolder.GetFolderAsync(relativeFolderPath);
            var files = await folder.GetFilesAsync();

            return files;
        }

        public async Task<bool> DeleteMagazineFolderAsync(string featureDirectoryName)
        {
            var doesFeatureDirectoryExistResult = await rootFolder.CheckExistsAsync(featureDirectoryName);
            if (doesFeatureDirectoryExistResult == ExistenceCheckResult.FolderExists)
            {
                var featureFolder = await rootFolder.GetFolderAsync(featureDirectoryName);
                await featureFolder.DeleteAsync();
                return true;
            }

            return false;
        }

        private async Task<IList<IFolder>> GetFolderDirectoriesAsync(string folderPath)
        {
            var folder = await rootFolder.GetFolderAsync(folderPath);
            var subFolders = await folder.GetFoldersAsync();
            return subFolders;
        }

        private async Task<IList<IFile>> GetFilesAsync(IFolder folder)
        {
            var files = await folder.GetFilesAsync();
            return files;
        }

        public async Task SaveFileAsync(byte[] file, string featureDirectoryName, string relativeFilePath)
        {
            var fileName = Path.GetFileName(relativeFilePath);

            var path =
                $"{rootFolder.Path}{DirectorySeperator}{featureDirectoryName}{DirectorySeperator}{relativeFilePath}";
            var directory = Path.GetDirectoryName(path);

            var directoryResult = await FileSystem.Current.GetFolderFromPathAsync(directory);
            if (directoryResult != null)
            {
                // Save file in existing directory.
                var folder = await FileSystem.Current.GetFolderFromPathAsync(directory);
                await SaveFileToValidPath(file, folder, fileName);
            }
            else
            {
                var pathSplitted = GetPathSplitted(relativeFilePath);
                if (pathSplitted.Count == 1)
                {
                    var folder = await FileSystem.Current.LocalStorage.CreateFolderAsync(featureDirectoryName, CreationCollisionOption.OpenIfExists);
                    await SaveFileToValidPath(file, folder, fileName);
                }
                else
                {
                    var folder = await rootFolder.CreateFolderAsync(directory, CreationCollisionOption.ReplaceExisting);
                    await SaveFileToValidPath(file, folder, fileName);
                }
            }
        }

        private async Task SaveFileToValidPath(byte[] file, IFolder folder, string fileName)
        {
            var createdFile = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            using (var fileStream = await createdFile.OpenAsync(FileAccess.ReadAndWrite))
            {
                await fileStream.WriteAsync(file, 0, file.Length);
            }
        }

        private List<string> GetPathSplitted(string filePath)
        {
            var levels = filePath.Split(DirectorySeperator);
            return levels?.ToList();
        }
    }
}
