using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using TressMontage.Client.Features.DataMagazine;
using TressMontage.Client.UWP.Features.DataMagazine;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Controls;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(CustomWebView), typeof(DisplayPDFRenderer))]
namespace TressMontage.Client.UWP.Features.DataMagazine
{
    public class DisplayPDFRenderer : ViewRenderer<CustomWebView, WebView>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<CustomWebView> e)
        {
            base.OnElementChanged(e);

            if (Element == null)
            {
                return;
            }

            if (Control == null)
            {
                SetNativeControl(new WebView());
            }

            if (e.NewElement != null)
            {
                var customWebView = Element as CustomWebView;

                // TODO: Here is a known memory leak.
                customWebView.PropertyChanged += OnPropertyChanged;
            }
        }

        private async void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var customWebView = sender as CustomWebView;
            if (customWebView.Uri != null && Control != null)
            {
                await LaunchPDFViewerAsync(customWebView.Uri);
            }
        }

        private async Task<IStorageFile> TryGetFileFromLocalFolder(string filePath)
        {
            var localFolder = ApplicationData.Current.LocalFolder;
            var relativePath = SubtractLocalPathFromFilePath(localFolder, filePath);
            var folderWithFile = await DigIntoLastFolderAsync(localFolder, relativePath);
            var fileName = Path.GetFileName(filePath);

            var file = await folderWithFile.GetFileAsync(fileName);

            return file;
        }

        private async Task<IStorageFolder> DigIntoLastFolderAsync(IStorageFolder startFolder, string relativePath)
        {
            var directory = Path.GetDirectoryName(relativePath);
            var folderNames = directory.Split(new string[] { "//" }, StringSplitOptions.None);

            var folder = startFolder;
            foreach (var folderName in folderNames)
            {
                folder = await folder.GetFolderAsync(folderName);
            }

            return folder;
        }

        private string SubtractLocalPathFromFilePath(StorageFolder localFolder, string filePath)
        {
            var relativePath = filePath.Remove(0, localFolder.Path.Length + 1);
            return relativePath;
        }

        /// <summary>
        /// A cooler way to display a pdf is this: 
        /// http://blog.pieeatingninjas.be/2016/02/06/displaying-pdf-files-in-a-uwp-app/
        /// </summary>
        private async Task LaunchPDFViewerAsync(string fileUrl)
        {
            Windows.System.LauncherOptions options = new Windows.System.LauncherOptions();
            options.ContentType = "application/pdf";

            var file = await TryGetFileFromLocalFolder(fileUrl);

            if (file != null)
            {
                // Launch the retrieved file
                var success = await Windows.System.Launcher.LaunchFileAsync(file);
                if (success)
                {
                    // File launched
                }
                else
                {
                    // File launch failed
                }
            }
        }
    }
}

/* FILEPICKER
    FileOpenPicker filePicker = new FileOpenPicker();
    filePicker.FileTypeFilter.Add(".pdf");
    filePicker.ViewMode = PickerViewMode.Thumbnail;
    filePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
    filePicker.SettingsIdentifier = "picker1";
    filePicker.CommitButtonText = "Open Pdf File";
    var file = await filePicker.PickSingleFileAsync();
 */
