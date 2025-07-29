using Our.Umbraco.Forms.Mailcoach.Models;

namespace Our.Umbraco.Forms.Mailcoach.Services;

public interface IMailcoachService
{
    Task<List<MailcoachEmailList>> GetEmailListsAsync();
}