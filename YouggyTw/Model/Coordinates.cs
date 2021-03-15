using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace YouggyTw.Model
{
    public class Coordinates
    {
        [JsonProperty("coordinates")]
        public List<double> CoordinatesList { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
