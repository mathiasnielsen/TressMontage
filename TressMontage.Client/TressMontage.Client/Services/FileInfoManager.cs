using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PCLStorage;
using TressMontage.Client.Core.Services;
using TressMontage.Entities;

namespace TressMontage.Client.Services
{
    public class FileInfoManager : IFileInfoManager
    {
        public async Task<List<Folder>> GetCurrentFolderPathsAsync(string currentFolderPath = "")
        {
            var subFolders = await GetFolderStructureAsync(currentFolderPath);
            var folders = subFolders.Select(x => new Folder() { Name = x.Name, Path = x.Path });

            return folders.ToList();
        }

        private async Task<IList<IFolder>> GetFolderStructureAsync(string currentFolderPath)
        {
            IFolder rootFolder = null;
            if (currentFolderPath == string.Empty)
            {
                rootFolder = FileSystem.Current.LocalStorage;
            }
            else
            {
                rootFolder = await FileSystem.Current.GetFolderFromPathAsync(currentFolderPath);
            }

            var subFolders = await rootFolder?.GetFoldersAsync();
            return subFolders;
        }

        private async Task<IList<IFile>> GetFilesAsync(IFolder folder)
        {
            var files = await folder.GetFilesAsync();
            return files;
        }
    }
}
