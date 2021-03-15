using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace YouggyTw.Model
{
    public class Description
    {
        [JsonProperty("urls")]
        public List<Url> Urls { get; set; }
    }
}
