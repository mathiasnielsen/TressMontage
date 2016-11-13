using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TressMontage.Client.Core.Services
{
    public interface IFileSystem
    {
        Task CreateDirectoryAsync(string path);

        Task<bool> DeleteFolderIfExistsAsync(string path);

        Task<bool> DeleteFileIfExistsAsync(string path);

        Task<bool> FileExistsAsync(string path);

        Task<bool> FolderExistsAsync(string path);

        Task<string> GetRootFolderPathAsync();

        /// <summary>
        /// Search for folders in folder and subfolders that 'startsWith' the given string.
        /// Ensure to build the search pattern so it contains at least one wildchar (*).
        /// </summary>
        Task<string[]> SearchFolderPathsInFolderAsync(string path, string searchPattern);

        /// <summary>
        /// Search for files in folder and subfolders that 'startsWith' the given string.
        /// Ensure to build the search pattern so it contains at least one wildchar (*).
        /// </summary>
        Task<string[]> SearchFilePathsInFolderAsync(string path, string searchPattern);

        Task<string[]> GetSubFolderPathsInFolderAsync(string path);

        Task WriteTextToFileAsync(string path, string value);

        Task<string> ReadTextFromFileAsync(string path);

        Task<byte[]> ReadBytesFromFileAsync(string attachmentPath);

        /// <summary>
        /// IMPORTANT: Remember to dispose the stream after use.
        /// </summary>
        Task<Stream> ReadStreamFromFileAsync(string path);

        /// <summary>
        /// IMPORTANT: Remember to dispose the stream after use.
        /// </summary>
        Task WriteStreamToFileAsync(string path, Stream stream);

        Task WriteBytesFileAsync(string path, byte[] data);

        /// <summary>
        /// IMPORTANT: iOS file time does not include "milliseconds".
        /// If file does not exist, we return DateTime.MinValue
        /// </summary>
        Task<DateTime> GetLastWriteTimeUtcAsync(string path);

        /// <summary>
        /// IMPORTANT: iOS file time does not include "milliseconds".
        /// </summary>
        Task SetWriteTimeUtcAsync(string path, DateTime creationTime);

        /// <summary>
        /// Get files paths normalized in "FormC" from a given folder if it exists. 
        /// If it doesn't exist we return null.
        /// </summary>
        Task<string[]> GetFilesFromFolderIfExistsAsync(string path);
    }
}