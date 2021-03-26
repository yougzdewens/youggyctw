using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace YouggyTw.Model
{
    public class Target
    {
        [JsonProperty("recipient_id")]
        public string RecipientId { get; set; }
    }
}
