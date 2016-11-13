using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TressMontage.Client.Core.Services;

namespace TressMontage.Client.iOS.Services
{
    public class FileSystem : IFileSystem
    {
        public FileSystem()
        {
        }

        public Task CreateDirectoryAsync(string path)
        {
            Directory.CreateDirectory(path);
            return Task.FromResult(0);
        }

        public async Task<bool> DeleteFolderIfExistsAsync(string path)
        {
            if (await FolderExistsAsync(path))
            {
                Directory.Delete(path, true);
                return true;
            }

            return false;
        }

        public async Task<bool> DeleteFileIfExistsAsync(string path)
        {
            if (await FileExistsAsync(path))
            {
                File.Delete(path);
                return true;
            }

            return false;
        }

        public Task<bool> FileExistsAsync(string path)
        {
            return Task.FromResult(File.Exists(path));
        }

        public Task<bool> FolderExistsAsync(string path)
        {
            return Task.FromResult(Directory.Exists(path));
        }

        public Task<string> GetRootFolderPathAsync()
        {
            return Task.FromResult(GetAppLibraryFolderPath());
        }

        public Task<string[]> SearchFolderPathsInFolderAsync(string path, string searchPattern)
        {
            var directories = Directory.GetDirectories(path, searchPattern, SearchOption.AllDirectories)
                .Select(x => x.Normalize(NormalizationForm.FormC))
                .ToArray();

            return Task.FromResult(directories);
        }

        public Task<string[]> SearchFilePathsInFolderAsync(string path, string searchPattern)
        {
            var directories = Directory.GetFiles(path, searchPattern, SearchOption.AllDirectories)
                .Select(x => x.Normalize(NormalizationForm.FormC))
                .ToArray();

            return Task.FromResult(directories);
        }

        public Task<string[]> GetSubFolderPathsInFolderAsync(string path)
        {
            var directories = Directory.GetDirectories(path)
                .Select(x => x.Normalize(NormalizationForm.FormC))
                .ToArray();

            return Task.FromResult(directories);
        }

        public Task<string> ReadTextFromFileAsync(string path)
        {
            return Task.FromResult(File.ReadAllText(path));
        }

        public Task<byte[]> ReadBytesFromFileAsync(string path)
        {
            return Task.FromResult(File.ReadAllBytes(path));
        }

        public Task<Stream> ReadStreamFromFileAsync(string path)
        {
            return Task.FromResult<Stream>(File.OpenRead(path));
        }

        public Task WriteTextToFileAsync(string path, string value)
        {
            File.WriteAllText(path, value);
            return Task.FromResult(0);
        }

        public async Task WriteStreamToFileAsync(string path, Stream stream)
        {
            using (var fileStream = File.OpenWrite(path))
            {
                await stream.CopyToAsync(fileStream);
            }
        }

        public async Task WriteBytesFileAsync(string path, byte[] data)
        {
            using (var fileStream = File.OpenWrite(path))
            {
                await fileStream.WriteAsync(data, 0, data.Length);
            }
        }

        /// <summary>
        /// We are notmalizing the strings to "FormC" because iOS file system otherwise would return
        /// it normalized as "FormD"
        /// </summary>
        public async Task<string[]> GetFilesFromFolderIfExistsAsync(string path)
        {
            if (await FolderExistsAsync(path))
            {
                return Directory.GetFiles(path)
                    .Select(x => x.Normalize(NormalizationForm.FormC))
                    .ToArray();
            }

            return default(string[]);
        }

        /// <summary>
        /// IMPORTANT: iOS file time does not include "milliseconds".
        /// If file does not exist, we return DateTime.MinValue
        /// </summary>
        public async Task<DateTime> GetLastWriteTimeUtcAsync(string path)
        {
            if (await FileExistsAsync(path))
            {
                return File.GetLastWriteTimeUtc(path);
            }

            return DateTime.MinValue;
        }

        /// <summary>
        /// IMPORTANT: 
        ///  * iOS file time does not include "milliseconds".
        ///  * Do not use File.SetCreationTime. To change file date one must SetLastWriteTime.
        /// </summary>
        public Task SetWriteTimeUtcAsync(string path, DateTime creationTime)
        {
            if (File.Exists(path))
            {
                File.SetLastWriteTimeUtc(path, creationTime);
            }

            return Task.FromResult(0);
        }

        /// <summary>
        /// For iOS we choose the Library directory.
        /// According to Xamarin documentation Library folder is a good place to 
        /// store files not created by the user.
        /// NOTE: The content is backed up by iTunes.
        /// <see cref="https://developer.xamarin.com/guides/ios/application_fundamentals/working_with_the_file_system/"/> for Xamarin documentation.
        /// </summary>
        private static string GetAppLibraryFolderPath()
        {
            var documentsFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            return Path.Combine(documentsFolderPath, "..", "Library");
        }
    }
}
