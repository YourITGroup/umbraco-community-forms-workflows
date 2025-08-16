namespace Umbraco.Community.Forms.Workflows.Configuration;

public class CommunityOptions
{
    public const string SectionName = "Community:Forms";
    
    public required Mailcoach Mailcoach { get; set; }
    public required MailChimp MailChimp { get; set; }
    public required CampaignMonitor CampaignMonitor { get; set; }
}
