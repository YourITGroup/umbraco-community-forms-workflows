using System.Text.Json.Serialization;

namespace Umbraco.Community.Forms.Workflows.Models.Mailcoach;

public class MailingListConfig
{
    [JsonPropertyName("domain")]
    public string? Domain { get; set; }

    [JsonPropertyName("token")]
    public string? Token { get; set; }

    [JsonPropertyName("listId")]
    public string? ListId { get; set; }
}