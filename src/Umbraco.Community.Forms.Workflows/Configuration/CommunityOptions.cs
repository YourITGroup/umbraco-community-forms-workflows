namespace Umbraco.Community.Forms.Workflows.Configuration;

public class CommunityOptions
{
    public const string SectionName = "Community:Forms";
    
    public Mailcoach Mailcoach { get; set; }
    public MailChimp MailChimp { get; set; }
}