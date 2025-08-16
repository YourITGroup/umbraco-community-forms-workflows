using System.Text.Json.Serialization;

namespace Umbraco.Community.Forms.Workflows.Models.CampaignMonitor;

public class MailingListConfig
{

    [JsonPropertyName("apiKey")]
    public string? ApiKey { get; set; }

    [JsonPropertyName("clientId")]
    public string? ClientId { get; set; }
    
    [JsonPropertyName("listId")]
    public string? ListId { get; set; }
}