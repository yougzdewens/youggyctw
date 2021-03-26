using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace YouggyTw.Model
{
    public class MessageData
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("entities")]
        public Entities Entities { get; set; }
    }
}
