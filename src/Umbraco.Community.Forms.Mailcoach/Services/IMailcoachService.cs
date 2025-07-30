using Umbraco.Community.Forms.Mailcoach.Models;

namespace Umbraco.Community.Forms.Mailcoach.Services;

public interface IMailcoachService
{
    /// <summary>
    /// Retrieve the available Mailcoach Lists
    /// </summary>
    /// <returns></returns>
    Task<List<MailcoachEmailList>> GetMailingListsAsync();

    /// <summary>
    /// Create a new subscriber in the specified Mailcoach list
    /// </summary>
    /// <param name="subscriber"></param>
    /// <param name="emailListId"></param>
    /// <returns></returns>
    Task<bool> AddSubscriber(MailcoachSubscriber subscriber, string emailListId);
}