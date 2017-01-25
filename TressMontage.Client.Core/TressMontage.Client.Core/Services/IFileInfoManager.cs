using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PCLStorage;
using TressMontage.Entities;

namespace TressMontage.Client.Core.Services
{
    public interface IFileInfoManager
    {
        Task<IFolder> RetrieveOrConstructFeatureRootDirectory(string featureDirectoryName);

        Task<IList<IFolder>> GetFoldersFromRootFoldersAsync(string featureDirectoryName);

        Task<IList<IFolder>> GetFoldersInDirectoryAsync(string featureDirectoryName, string relativeFolderPath);

        Task<IList<IFile>> GetFilesDirectoriesInFolderAsync(string relativeFolderPath);

        Task<bool> DeleteMagazineFolderAsync(string featureDirectoryName);

        Task SaveFileAsync(byte[] file, string featureDirectoryName, string relativeFilePath);
    }
}
