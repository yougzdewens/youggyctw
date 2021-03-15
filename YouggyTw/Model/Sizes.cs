using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace YouggyTw.Model
{   
    public class Sizes
    {
        [JsonProperty("thumb")]
        public Thumb Thumb { get; set; }

        [JsonProperty("medium")]
        public Medium Medium { get; set; }

        [JsonProperty("small")]
        public Small Small { get; set; }

        [JsonProperty("large")]
        public Large Large { get; set; }
    }
}