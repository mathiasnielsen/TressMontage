using Newtonsoft.Json;
using TressMontage.Entities.JsonConverters;
using TressMontage.Utilities;
using System.Data

namespace TressMontage.Entities
{
    public class FileInfoDTO
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        ////[JsonConverter(typeof(FileTypeConverter))]
        [JsonProperty("type")]
        public FileTypes Type { get; set; }
    }
}
