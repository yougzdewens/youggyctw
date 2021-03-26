using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace YouggyTw.Model
{
    public class EventTwitter
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("created_timestamp")]
        public string CreatedTimestamp { get; set; }

        [JsonProperty("message_create")]
        public MessageCreate MessageCreate { get; set; }

        public EventTwitter()
        {
            MessageCreate = new MessageCreate();
        }
    }
}
