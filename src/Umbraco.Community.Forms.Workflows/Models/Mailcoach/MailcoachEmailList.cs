using System.Text.Json.Serialization;

namespace Umbraco.Community.Forms.Workflows.Models.Mailcoach;

public class EmailList
{
    [JsonPropertyName("uuid")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }
}

public class EmailListsResponse
{
    [JsonPropertyName("data")]
    public List<EmailList> Data { get; set; } = [];

    [JsonPropertyName("links")]
    public Links? Links { get; set; }

    [JsonPropertyName("meta")]
    public Meta? Meta { get; set; }
}

public class Links
{
    [JsonPropertyName("first")]
    public string? First { get; set; }

    [JsonPropertyName("last")]
    public string? Last { get; set; }

    [JsonPropertyName("prev")]
    public string? Prev { get; set; }

    [JsonPropertyName("next")]
    public string? Next { get; set; }
}

public class Meta
{
    [JsonPropertyName("current_page")]
    public int CurrentPage { get; set; }

    [JsonPropertyName("from")]
    public int? From { get; set; }

    [JsonPropertyName("last_page")]
    public int LastPage { get; set; }

    [JsonPropertyName("per_page")]
    public int PerPage { get; set; }

    [JsonPropertyName("to")]
    public int? To { get; set; }

    [JsonPropertyName("total")]
    public int Total { get; set; }
}