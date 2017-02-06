using Newtonsoft.Json;
using TressMontage.Utilities;

namespace TressMontage.Entities
{
    public class FileInfoDTO
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("type")]
        public FileTypes Type { get; set; }
    }
}
