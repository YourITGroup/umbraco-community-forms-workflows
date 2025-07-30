namespace Umbraco.Community.Forms.Mailcoach.Configuration;

public class MailcoachOptions
{
    public const string SectionName = "Mailcoach";
    
    public string ApiDomain { get; set; } = string.Empty;
    public string ApiToken { get; set; } = string.Empty;
}