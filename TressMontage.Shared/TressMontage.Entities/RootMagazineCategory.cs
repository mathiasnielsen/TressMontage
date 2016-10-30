using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TressMontage.Entities
{
    public class RootMagazineCategory
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("magazine_sub_categories")]
        public List<MagazineCategory> MagazinesSubCategories { get; set; }
    }
}
