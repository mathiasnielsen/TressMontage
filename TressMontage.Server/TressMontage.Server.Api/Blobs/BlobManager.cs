using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using TressMontage.Entities;
using TressMontage.Utilities;

namespace TressMontage.Server.Api.Blobs
{
    public class BlobManager
    {
        private const string ConnectionString = "DefaultEndpointsProtocol=https;AccountName=tressmontagestorage;AccountKey=NfP2JeN3yg3zorKs+m0LHr0qrzO1QgT6gU9i+JmErY4P2PZi/v2PasqqIr/pVwd/jRAe99i13rYLFj6V8Hko7Q==";
        private const string ContainerName = "data-magazines";

        public async Task<bool> UploadBlobIntoContainer(FileDTO fileDto)
        {
            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConnectionString);

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve reference to a previously created container.
            CloudBlobContainer container = blobClient.GetContainerReference(ContainerName);

            // Retrieve reference to a blob named "myblob".
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileDto.FileInfo.DirectoryWithName);

            // Create or overwrite the "myblob" blob with contents from a local file.
            await blockBlob.UploadFromByteArrayAsync(fileDto.Data, 0, fileDto.Data.Length);
            return true;
        }

        public async Task<byte[]> GetBlobAsync(string blobName)
        {
            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConnectionString);

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve reference to a previously created container.
            CloudBlobContainer container = blobClient.GetContainerReference(ContainerName);

            CloudBlob blob = container.GetBlobReference(blobName);

            ////// Save blob contents to a file.
            using (var fileStream = new MemoryStream())
            {
                await blob.DownloadToStreamAsync(fileStream);
                return fileStream.ToArray();
            }
        }

        public IEnumerable<CloudBlockBlob> GetBlobsInContainer()
        {
            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConnectionString);

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve reference to a previously created container.
            CloudBlobContainer container = blobClient.GetContainerReference(ContainerName);

            var blobs = container.ListBlobs(null, true, BlobListingDetails.All).Cast<CloudBlockBlob>();
            return blobs;
        }

        public async Task<List<IListBlobItem>> ListBlobsInContainerAsync()
        {
            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConnectionString);

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve reference to a previously created container.
            CloudBlobContainer container = blobClient.GetContainerReference(ContainerName);

            await container.CreateIfNotExistsAsync();

            var results = await ListBlobsSegmentedInFlatListing(container);
            var list = results?.ToList();

            return list;
        }

        public async Task<IEnumerable<IListBlobItem>> ListBlobsSegmentedInFlatListing(CloudBlobContainer container)
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

        public FileDirective GetFileInfoFromBlob(CloudBlockBlob blob)
        {
            var fileInfoDto = new FileDirective();

            fileInfoDto.BlobPath = blob.Name;

            return fileInfoDto;
        }
    }
}