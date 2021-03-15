using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace YouggyTw.Model
{  
    public class AdditionalMediaInfo
    {
        [JsonProperty("monetizable")]
        public bool Monetizable { get; set; }
    }
}
