using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace TressMontage.Administration.Cmd
{
    class Program
    {
        private const string StorageConnectionString = nameof(StorageConnectionString);
        private const string ContainerName = "data-magazines";

        static void Main(string[] args)
        {
            StartAsync();
            Console.ReadLine();
        }

        private async static void StartAsync()
        {
            Console.WriteLine("### Before synching:");
            await ListBlobsInContainerAsync();

            var currentDirectory = Directory.GetCurrentDirectory();
#if DEBUG
            var debugFolder = Path.GetDirectoryName(currentDirectory);
            var projectFolder = Path.GetDirectoryName(debugFolder);
            currentDirectory = projectFolder + "\\TestData";
#endif
            await SynchDirectoryDataAsync(currentDirectory);

            Console.WriteLine("### After synching:");
            await ListBlobsInContainerAsync();

#if DEBUG
            Console.WriteLine("\nPress enter to close...");
#endif
        }

        private static async Task SynchDirectoryDataAsync(string currentDirectory)
        {
            var allFiles = Directory.GetFiles(currentDirectory, "*txt", SearchOption.AllDirectories);
            foreach (var file in allFiles)
            {
                var uploadResult = await UploadFileAsync(file, currentDirectory);
            }
        }

        private static async Task<bool> UploadFileAsync(string filePath, string rootPath)
        {
            await UploadBlobIntoContainer(filePath, rootPath);
            return true;
        }

        private static async Task UploadBlobIntoContainer(string filePath, string rootPath)
        {
            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting(StorageConnectionString));

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve reference to a previously created container.
            CloudBlobContainer container = blobClient.GetContainerReference(ContainerName);

            var fileInfo = new FileInfo(filePath);
            var filePathWithRelativeDirectory = fileInfo.FullName.Remove(0, rootPath.Length + 1);

            // Retrieve reference to a blob named "myblob".
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(filePathWithRelativeDirectory);
            ////CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileInfo.Name);

            // Create or overwrite the "myblob" blob with contents from a local file.
            using (var fileStream = File.OpenRead(filePath))
            {
                await blockBlob.UploadFromStreamAsync(fileStream);
            }
        }

        private void PrintCurrentDirectoryStruture(string currentDirectory)
        {
            var subDirectories = Directory.GetDirectories(currentDirectory);
            foreach (var directory in subDirectories)
            {
                var folderInfo = new DirectoryInfo(directory);
                Console.WriteLine(folderInfo.Name);
            }

            var files = Directory.GetFiles(currentDirectory);
            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                Console.WriteLine(fileInfo.Name);
            }
        }

        private static async Task ListBlobsInContainerAsync()
        {
            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting(StorageConnectionString));

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve reference to a previously created container.
            CloudBlobContainer container = blobClient.GetContainerReference(ContainerName);

            await container.CreateIfNotExistsAsync();

            var results = await ListBlobsSegmentedInFlatListing(container);
        }

        public static async Task<IEnumerable<IListBlobItem>> ListBlobsSegmentedInFlatListing(CloudBlobContainer container)
        {
            //List blobs to the console window, with paging.
            Console.WriteLine("List blobs in pages:");

            int i = 0;
            BlobContinuationToken continuationToken = null;
            BlobResultSegment resultSegment = null;

            //Call ListBlobsSegmentedAsync and enumerate the result segment returned, while the continuation token is non-null.
            //When the continuation token is null, the last page has been returned and execution can exit the loop.
            do
            {
                //This overload allows control of the page size. You can return all remaining results by passing null for the maxResults parameter,
                //or by calling a different overload.
                resultSegment = await container.ListBlobsSegmentedAsync("", true, BlobListingDetails.All, 10, continuationToken, null, null);
                if (resultSegment.Results.Count<IListBlobItem>() > 0) { Console.WriteLine("Page {0}:", ++i); }
                foreach (var blobItem in resultSegment.Results)
                {
                    Console.WriteLine("\t{0}", blobItem.StorageUri.PrimaryUri);
                }

                Console.WriteLine();

                //Get the continuation token.
                continuationToken = resultSegment.ContinuationToken;
            }
            while (continuationToken != null);

            return resultSegment.Results;
        }
    }
}
