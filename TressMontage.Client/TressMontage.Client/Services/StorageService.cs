using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using PCLStorage;
using TressMontage.Client.Core.Services;

namespace TressMontage.Client.Services
{
    public class StorageService : IStorageService
    {
        /// <summary>
        /// Saving Values to the Storage...
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Key"></param>
        /// <param name="ValueToSave"></param>
        /// <returns></returns>
        public async Task Save_Value<T>(String Key, T ValueToSave)
        {
            XDocument doc = new XDocument();
            using (var writer = doc.CreateWriter())
            {
                var serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(writer, ValueToSave);
            }

            IFolder rootFolder = FileSystem.Current.LocalStorage;
            IFolder folder = await rootFolder.CreateFolderAsync("Cache",
                CreationCollisionOption.OpenIfExists);
            IFile file = await folder.CreateFileAsync(Key + ".txt",
                CreationCollisionOption.ReplaceExisting);

            await file.WriteAllTextAsync(doc.ToString());
        }

        /// <summary>
        /// Reading Values from the Storage...
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Key"></param>
        /// <returns></returns>
        public async Task<T> Get_Value<T>(String Key)
        {
            IFolder rootFolder = FileSystem.Current.LocalStorage;
            IFolder folder = await rootFolder.CreateFolderAsync("Cache",
                CreationCollisionOption.OpenIfExists);

            ExistenceCheckResult isFileExisting = await folder.CheckExistsAsync(Key + ".txt");

            if (!isFileExisting.ToString().Equals("NotFound"))
            {
                try
                {
                    IFile file = await folder.CreateFileAsync(Key + ".txt",
                    CreationCollisionOption.OpenIfExists);

                    String languageString = await file.ReadAllTextAsync();

                    XmlSerializer oXmlSerializer = new XmlSerializer(typeof(T));
                    return (T)oXmlSerializer.Deserialize(new StringReader(languageString));
                }
                catch (Exception ex)
                {
                    if (Debugger.IsAttached)
                    {
                        Debug.WriteLine($"Error in deleting document: {ex.Message}");
                    }

                    return default(T);
                }
            }

            return default(T);
        }

        /// <summary>
        /// Delete any value from the Storage...
        /// </summary>
        /// <param name="Key"></param>
        public async void Delete_Value(String Key)
        {
            IFolder rootFolder = FileSystem.Current.LocalStorage;
            IFolder folder = await rootFolder.CreateFolderAsync("Cache",
                CreationCollisionOption.OpenIfExists);

            ExistenceCheckResult isFileExisting = await folder.CheckExistsAsync(Key + ".txt");

            if (!isFileExisting.ToString().Equals("NotFound"))
            {
                try
                {
                    IFile file = await folder.CreateFileAsync(Key + ".txt",
                    CreationCollisionOption.OpenIfExists);

                    await file.DeleteAsync();
                }
                catch (Exception ex)
                {
                    if (Debugger.IsAttached)
                    {
                        Debug.WriteLine($"Error in deleting document: {ex.Message}");
                    }
                }
            }
        }
    }
}
