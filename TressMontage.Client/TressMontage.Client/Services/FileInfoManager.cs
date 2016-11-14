using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PCLStorage;
using TressMontage.Client.Core.Services;
using TressMontage.Entities;
using TressMontage.Client.Extensions;
using System.IO;

namespace TressMontage.Client.Services
{
    public class FileInfoManager : IFileInfoManager
    {
        private string LocalStorageUrl;

        public FileInfoManager()
        {
            LocalStorageUrl = FileSystem.Current.LocalStorage.Path;
        }

        public async Task<List<Folder>> GetFoldersAsync(string relativeFolderPath)
        {
            var subFolders = await GetFolderDirectoriesAsync(relativeFolderPath);
            var folders = subFolders.Select(x => new Folder() { Name = x.Name, Path = x.Path });

            return folders.ToList();
        }

        public async Task<List<FileInfo>> GetFilesDirectoriesInFolderAsync(string relativeFolderPath)
        {
            var path = LocalStorageUrl + "\\" + relativeFolderPath;

            var folder = await FileSystem.Current.GetFolderFromPathAsync(path);
            var files = await folder.GetFilesAsync();

            return files.Select(file => new FileInfo() { Name = file.Name, Path = file.Path, Type = file.Name.GetFileType() }).ToList();
        }

        private async Task<IList<IFolder>> GetFolderDirectoriesAsync(string folderPath)
        {
            var rootFolder = await FileSystem.Current.LocalStorage.GetFolderAsync(folderPath);
            var subFolders = await rootFolder?.GetFoldersAsync();
            return subFolders;
        }

        private async Task<IList<IFile>> GetFilesAsync(IFolder folder)
        {
            var files = await folder.GetFilesAsync();
            return files;
        }

        public async Task SaveFileAsync(byte[] file, string rootFolder, string relativeFilePath)
        {
            var fileName = Path.GetFileName(relativeFilePath);

            var path = $"{LocalStorageUrl}\\{rootFolder}\\{relativeFilePath}";
            var directory = Path.GetDirectoryName(path);
            var directoryResult = await FileSystem.Current.GetFolderFromPathAsync(directory);
            if (directoryResult != null)
            {
                // Save file in existing directory.
                var folder = await FileSystem.Current.GetFolderFromPathAsync(directory);
                await SaveFileToValidPath(file, folder, fileName);
            }

            var pathSplitted = GetPathSplitted(relativeFilePath);
            if (pathSplitted.Count == 1)
            {
                var folder = await FileSystem.Current.LocalStorage.CreateFolderAsync(rootFolder, CreationCollisionOption.OpenIfExists);
                await SaveFileToValidPath(file, folder, fileName);
            }
            else
            {
                var folder = await ConstructDirectoryWithSubDirectoriesAsync(rootFolder, pathSplitted);
                await SaveFileToValidPath(file, folder, fileName);
            }
        }

        private async Task<IFolder> ConstructDirectoryWithSubDirectoriesAsync(string rootFolder, List<string> directories)
        {
            directories.RemoveAt(directories.Count - 1);

            var url = $"{LocalStorageUrl}\\{rootFolder}\\";
            var finalFolder = await FileSystem.Current.LocalStorage.GetFolderAsync(rootFolder);
            for (int index = 0; index < directories.Count; index++)
            {
                var newUrl = $@"{url}{directories[index]}\";
                if (await FileSystem.Current.GetFolderFromPathAsync(newUrl) == null)
                {
                    // If not exisiting then create one.
                    var test = await finalFolder.CreateFolderAsync(directories[index], CreationCollisionOption.ReplaceExisting);
                }

                url = newUrl;
                finalFolder = await FileSystem.Current.GetFolderFromPathAsync(url);
            }

            return finalFolder;
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
            var levels = filePath.Split('/');
            return levels?.ToList();
        }
    }
}
