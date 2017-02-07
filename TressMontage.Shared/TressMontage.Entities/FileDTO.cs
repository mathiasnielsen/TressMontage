using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TressMontage.Entities
{
    public class FileDTO
    {
        [JsonProperty("filedirective")]
        public FileDirective FileInfo { get; set; }

        [JsonProperty("data")]
        public byte[] Data { get; set; }
    }
}
