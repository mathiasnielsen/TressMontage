using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PCLStorage;
using TressMontage.Client.Core.Services;
using TressMontage.Utilities.Extensions;

namespace TressMontage.Client.Services
{
    public class FileInfoManager : IFileInfoManager
    {
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

        public async Task<IList<IFile>> GetFileDirectoriesInRootFolderAsync(string rootFolderName)
        {
            var folder = await rootFolder.GetFolderAsync(rootFolderName);
            var files = await folder.GetFilesAsync();

            return files;
        }

        public async Task<IList<IFile>> GetFilesDirectoriesInFolderAsync(string relativeFolderPath)
        {
            var folder = await FileSystem.Current.GetFolderFromPathAsync(relativeFolderPath);
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

            var path = Path.Combine(rootFolder.Path, featureDirectoryName, relativeFilePath);
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
                if (IsPathRooted(relativeFilePath))
                {
                    var featuredDirectory = Path.Combine(featureDirectoryName, relativeFilePath);
                    var folder = await TryGetFolderAsync(featuredDirectory);

                    ////var classicDirectory = GetDirectoryAsClassPath(relativeFilePath);
                    ////var combinedDirectory = classicDirectory.GetPlatformSpecificPath();

                    ////var folder = await FileSystem.Current.LocalStorage.CreateFolderAsync(combinedDirectory, CreationCollisionOption.OpenIfExists);
                    await SaveFileToValidPath(file, folder, fileName);
                }
                else
                {
                    var folder = await rootFolder.CreateFolderAsync(directory, CreationCollisionOption.ReplaceExisting);
                    await SaveFileToValidPath(file, folder, fileName);
                }
            }
        }

        private async Task<IFolder> TryGetFolderAsync(string directory)
        {
            try
            {
                var classicPath = GetDirectoryAsClassPath(directory);
                var folder = await rootFolder.CreateFolderAsync(classicPath, CreationCollisionOption.ReplaceExisting);
                return folder;
            }
            catch(Exception ex)
            {
                throw new Exception();
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

        private bool IsPathRooted(string filePath)
        {
            var classicPath = ConvertToClassicPath(filePath);
            var directoryName = Path.GetDirectoryName(classicPath);

            return string.IsNullOrEmpty(directoryName) == false;
        }

        private string ConvertToClassicPath(string path)
        {
            var splittedPath = path.Split('\\');
            var classicPath = string.Join("/", splittedPath);

            return classicPath;
        }

        private string GetDirectoryAsClassPath(string path)
        {
            var classicPath = ConvertToClassicPath(path);
            var classicDirectory = Path.GetDirectoryName(classicPath);

            return classicDirectory;
        }
    }
}
