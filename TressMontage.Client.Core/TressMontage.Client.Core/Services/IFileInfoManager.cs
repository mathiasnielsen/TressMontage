using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TressMontage.Entities;

namespace TressMontage.Client.Core.Services
{
    public interface IFileInfoManager
    {
        Task<List<Folder>> GetFoldersFromFullPath(string fullPath);

        Task<List<FileInfo>> GetFilesDirectoriesInFolderFromFullPathAsync(string fullPath);

        Task<List<Folder>> GetFoldersAsync(string relativeFolderPath);

        Task<List<FileInfo>> GetFilesDirectoriesInFolderAsync(string relativeFolderPath);

        Task SaveFileAsync(byte[] file, string rootFolder, string filePath);
    }
}
