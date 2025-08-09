using System.Text.Json.Serialization;

namespace Umbraco.Community.Forms.Workflows.Models.Mailcoach;

public class Subscriber
{
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("first_name")]
    public string? FirstName { get; set; }

    [JsonPropertyName("last_name")]
    public string? LastName { get; set; }

    [JsonPropertyName("tags")]
    public string[]? Tags { get; set; }

    [JsonPropertyName("skip_confirmation")]
    public bool SkipConfirmation { get; set; } = false;

    [JsonPropertyName("extra_attributes")]
    public Dictionary<string, object>? ExtraAttributes { get; set; }
}