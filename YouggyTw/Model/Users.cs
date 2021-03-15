using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace YouggyTw.Model
{
    public class Users
    {
        [JsonProperty("users")]
        public List<User> UserList { get; set; }

        [JsonProperty("next_cursor")]
        public long NextCursor { get; set; }

        [JsonProperty("next_cursor_str")]
        public string NextCursorStr { get; set; }

        [JsonProperty("previous_cursor")]
        public long PreviousCursor { get; set; }

        [JsonProperty("previous_cursor_str")]
        public string PreviousCursorStr { get; set; }

        [JsonProperty("total_count")]
        public object TotalCount { get; set; }
    }
}
