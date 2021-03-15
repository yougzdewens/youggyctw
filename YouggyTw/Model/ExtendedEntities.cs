using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace YouggyTw.Model
{
    public class ExtendedEntities
    {
        [JsonProperty("media")]
        public List<Medium> Media { get; set; }
    }
}
