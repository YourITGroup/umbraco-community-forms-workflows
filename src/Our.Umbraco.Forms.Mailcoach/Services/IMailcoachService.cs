using Our.Umbraco.Forms.Mailcoach.Models;

namespace Our.Umbraco.Forms.Mailcoach.Services;

public interface IMailcoachService
{
    /// <summary>
    /// Retrieve the available Mailcoach Lists
    /// </summary>
    /// <returns></returns>
    Task<List<MailcoachEmailList>> GetEmailListsAsync();

    /// <summary>
    /// Create a new subscriber in the specified Mailcoach list
    /// </summary>
    /// <param name="subscriber"></param>
    /// <param name="emailListId"></param>
    /// <returns></returns>
    Task<bool> AddSubscriber(MailcoachSubscriber subscriber, string emailListId);
}