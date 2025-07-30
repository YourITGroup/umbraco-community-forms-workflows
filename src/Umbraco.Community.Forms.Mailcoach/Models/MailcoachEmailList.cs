using Newtonsoft.Json;

namespace Umbraco.Community.Forms.Mailcoach.Models;

public class MailcoachEmailList
{
    [JsonProperty("uuid")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonProperty("updated_at")]
    public DateTime UpdatedAt { get; set; }
}

public class MailcoachEmailListsResponse
{
    [JsonProperty("data")]
    public List<MailcoachEmailList> Data { get; set; } = new();

    [JsonProperty("links")]
    public MailcoachLinks? Links { get; set; }

    [JsonProperty("meta")]
    public MailcoachMeta? Meta { get; set; }
}

public class MailcoachLinks
{
    [JsonProperty("first")]
    public string? First { get; set; }

    [JsonProperty("last")]
    public string? Last { get; set; }

    [JsonProperty("prev")]
    public string? Prev { get; set; }

    [JsonProperty("next")]
    public string? Next { get; set; }
}

public class MailcoachMeta
{
    [JsonProperty("current_page")]
    public int CurrentPage { get; set; }

    [JsonProperty("from")]
    public int? From { get; set; }

    [JsonProperty("last_page")]
    public int LastPage { get; set; }

    [JsonProperty("per_page")]
    public int PerPage { get; set; }

    [JsonProperty("to")]
    public int? To { get; set; }

    [JsonProperty("total")]
    public int Total { get; set; }
}