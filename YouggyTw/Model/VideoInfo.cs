using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace YouggyTw.Model
{
    public class VideoInfo
    {
        [JsonProperty("aspect_ratio")]
        public List<int> AspectRatio { get; set; }

        [JsonProperty("duration_millis")]
        public int DurationMillis { get; set; }

        [JsonProperty("variants")]
        public List<Variant> Variants { get; set; }
    }
}
