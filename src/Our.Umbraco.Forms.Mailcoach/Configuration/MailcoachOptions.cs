namespace Our.Umbraco.Forms.Mailcoach.Configuration;

public class MailcoachOptions
{
    public const string SectionName = "Mailcoach";
    
    public string ApiEndpoint { get; set; } = string.Empty;
    public string ApiToken { get; set; } = string.Empty;
}