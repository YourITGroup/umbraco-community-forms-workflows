using System.Text.Json.Serialization;

namespace Umbraco.Community.Forms.Workflows.Models.MailChimp;

public class MailingListConfig
{

    [JsonPropertyName("apiKey")]
    public string? ApiKey { get; set; }
    [JsonPropertyName("listId")]
    public string? ListId { get; set; }
}