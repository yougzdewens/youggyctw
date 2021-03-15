using System;
using System.Collections.Generic;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public partial class Messages
{
    [JsonProperty("events")]
    public Event[] Events { get; set; }
}

public partial class Event
{
    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("created_timestamp")]
    public string CreatedTimestamp { get; set; }

    [JsonProperty("message_create")]
    public MessageCreate MessageCreate { get; set; }
}

public partial class MessageCreate
{
    [JsonProperty("target")]
    public Target Target { get; set; }

    [JsonProperty("sender_id")]
    public string SenderId { get; set; }

    [JsonProperty("message_data")]
    public MessageData MessageData { get; set; }
}

public partial class MessageData
{
    [JsonProperty("text")]
    public string Text { get; set; }

    [JsonProperty("entities")]
    public Entities Entities { get; set; }
}

public partial class Entities
{
    [JsonProperty("hashtags")]
    public object[] Hashtags { get; set; }

    [JsonProperty("symbols")]
    public object[] Symbols { get; set; }

    [JsonProperty("user_mentions")]
    public object[] UserMentions { get; set; }

    [JsonProperty("urls")]
    public object[] Urls { get; set; }
}

public partial class Target
{
    [JsonProperty("recipient_id")]
    public string RecipientId { get; set; }
}