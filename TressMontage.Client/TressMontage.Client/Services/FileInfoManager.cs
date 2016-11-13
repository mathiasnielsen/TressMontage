using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PCLStorage;
using TressMontage.Client.Core.Services;
using TressMontage.Entities;
using TressMontage.Client.Extensions;

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

        public async Task SaveFileAsync(byte[] file, string filePath)
        {
            var folder = await FileSystem.Current.LocalStorage.CreateFolderAsync(filePath, CreationCollisionOption.OpenIfExists);

            var fileName = GetPathSplitted(filePath).LastOrDefault();
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
