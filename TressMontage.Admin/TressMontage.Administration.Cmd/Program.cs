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
            await ListBlobsInContainerAsync();

            var currentDirectory = Directory.GetCurrentDirectory();
#if DEBUG
            var debugFolder = Path.GetDirectoryName(currentDirectory);
            var projectFolder = Path.GetDirectoryName(debugFolder);
            currentDirectory = projectFolder + "\\TestData";
#endif
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

#if DEBUG
            Console.WriteLine("\nPress enter to close...");
#endif
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

            await ListBlobsSegmentedInFlatListing(container);
        }

        public static async Task ListBlobsSegmentedInFlatListing(CloudBlobContainer container)
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
        }
    }
}
