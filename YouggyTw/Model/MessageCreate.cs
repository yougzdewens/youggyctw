using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace YouggyTw.Model
{
    public class MessageCreate
    {
        [JsonProperty("target")]
        public Target Target { get; set; }

        [JsonProperty("sender_id")]
        public string SenderId { get; set; }

        [JsonProperty("message_data")]
        public MessageData MessageData { get; set; }

        public MessageCreate()
        {
            Target = new Target();
            MessageData = new MessageData();
        }
    }
}
