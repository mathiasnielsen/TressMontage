using System.IO;
using Newtonsoft.Json;
using TressMontage.Utilities;

namespace TressMontage.Entities
{
    public class FileDirective
    {
        [JsonProperty("blobPath")]
        public string BlobPath { get; set; }

        public DirectiveTypes Type { get; set; }

        public string Name { get { return Path.GetFileNameWithoutExtension(BlobPath); } }

        public string Directory { get { return Path.GetDirectoryName(BlobPath); } }

        public string Extension { get { return Path.GetExtension(BlobPath); } }

        public string ExtensionNoDot
        {
            get
            {
                if (Extension.Contains("."))
                {
                    var extensionNoDot = Extension.Remove(0, 1);
                    return extensionNoDot;
                }

                return Extension;
            }
        }

        public string DirectoryWithName
        {
            get
            {
                return Path.Combine(Directory, Name);
            }
        }
    }
}
