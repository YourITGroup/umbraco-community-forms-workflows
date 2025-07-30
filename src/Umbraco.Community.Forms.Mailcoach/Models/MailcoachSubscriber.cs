using Newtonsoft.Json;

namespace Umbraco.Community.Forms.Mailcoach.Models;

public class MailcoachSubscriber
{
    [JsonProperty("email")]
    public string Email { get; set; } = string.Empty;

    [JsonProperty("first_name")]
    public string? FirstName { get; set; }

    [JsonProperty("last_name")]
    public string? LastName { get; set; }

    [JsonProperty("tags")]
    public string[]? Tags { get; set; }

    [JsonProperty("skip_confirmation")]
    public bool SkipConfirmation { get; set; } = false;

    [JsonProperty("extra_attributes")]
    public Dictionary<string, object>? ExtraAttributes { get; set; }
}