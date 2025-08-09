using Umbraco.Community.Forms.Workflows.Models.Mailcoach;

namespace Umbraco.Community.Forms.Workflows.Services;

public interface IMailcoachService
{
    /// <summary>
    /// Retrieve the available Mailcoach Lists
    /// </summary>
    /// <returns></returns>
    Task<List<EmailList>> GetMailingListsAsync();

    /// <summary>
    /// Create a new subscriber in the specified Mailcoach list
    /// </summary>
    /// <param name="subscriber"></param>
    /// <param name="emailListId"></param>
    /// <returns></returns>
    Task<bool> AddSubscriber(Subscriber subscriber, string emailListId);

    /// <summary>
    /// Configure Mailcoach to use the supplied domain using Configuration as a fallback
    /// </summary>
    /// <param name="domain"></param>
    void ConfigureBaseAddress(string? domain = null);

    /// <summary>
    /// Configure Mailcoach to use the supplied token using Configuration as a fallback
    /// </summary>
    /// <param name="token"></param>
    void ConfigureApiToken(string? token = null);
}