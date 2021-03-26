using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace YouggyTw.Model
{
    public class EventsTwitter
    {
        [JsonProperty("events")]
        public List<EventTwitter> Events { get; set; }

        [JsonProperty("next_cursor")]
        public long NextCursor { get; set; }
    }
}
