using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace YouggyTw.Model
{   
    public class BoundingBox
    {
        [JsonProperty("coordinates")]
        public List<List<List<double>>> Coordinates { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
