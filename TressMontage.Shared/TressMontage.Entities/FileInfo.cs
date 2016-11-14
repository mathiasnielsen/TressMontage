using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TressMontage.Entities
{
    public class FileInfo
    {
        public string Name { get; set; }

        public string Path { get; set; }

        public string Type { get; set; }

        [JsonIgnore]
        public FileType FileType
        {
            get
            {
                var fileType = FileType.unknown;
                var isSucces = Enum.TryParse(Type, out fileType);
                return isSucces ? fileType : FileType.unknown;
            }
        }
    }
}
