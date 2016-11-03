using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TressMontage.Client.Core.Common;
using TressMontage.Client.Core.Features.Base;
using TressMontage.Client.Core.Services;
using TressMontage.Entities;

namespace TressMontage.Client.Core.Features.DataMagazine
{
    public class DataMagazineViewModel : BindableViewModelBase
    {
        private readonly IFileInfoManager fileInfoManager;
        private readonly INavigationService navigationService;

        private List<Folder> folders;

        public DataMagazineViewModel(INavigationService navigationService, IFileInfoManager fileInfoManager)
        {
            this.navigationService = navigationService;
            this.fileInfoManager = fileInfoManager;

            GoToFolderCommand = new RelayCommand<Folder>(GoToFolder);
        }

        public RelayCommand<Folder> GoToFolderCommand { get; set; }

        public List<Folder> Folders
        {
            get { return folders; }
            set { Set(ref folders, value); }
        }

        public override async Task OnViewInitialized()
        {
            Folders = await fileInfoManager.GetCurrentFolderPathsAsync();

            await base.OnViewInitialized();
        }

        private void GoToFolder(Folder obj)
        {
            navigationService.NavigateToHome();
        }

        private async Task GetBlobData()
        {
            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=your_account_name_here;AccountKey=your_account_key_here");

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve reference to a previously created container.
            CloudBlobContainer container = blobClient.GetContainerReference("mycontainer");

            // Create the container if it doesn't already exist.
            await container.CreateIfNotExistsAsync();

            // Retrieve reference to a blob named "myblob".
            CloudBlockBlob blockBlob = container.GetBlockBlobReference("myblob");

            // Create the "myblob" blob with the text "Hello, world!"
            await blockBlob.UploadTextAsync("Hello, world!");
        }
    }
}
