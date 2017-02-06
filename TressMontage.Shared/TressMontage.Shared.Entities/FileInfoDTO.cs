using Newtonsoft.Json;
using System.Data.Entity;

namespace TressMontage.Entities
{
    public class FileInfoDTO
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        ////[JsonProperty("path")]
        ////public string Path { get; set; }

        ////////[JsonConverter(typeof(FileTypeConverter))]
        ////[JsonProperty("type")]
        ////public FileTypes Type { get; set; }
    }
}
