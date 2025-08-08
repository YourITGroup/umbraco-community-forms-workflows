using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Umbraco.Community.Forms.Mailcoach.Configuration;
using Umbraco.Community.Forms.Mailcoach.Models;

namespace Umbraco.Community.Forms.Mailcoach.Services;

public class MailcoachService : IMailcoachService
{
    private readonly MailcoachOptions mailcoachOptions;
    private readonly HttpClient httpClient;
    private readonly ILogger<MailcoachService> logger;

    public MailcoachService(
        HttpClient httpClient,
        ILogger<MailcoachService> logger,
        IOptions<MailcoachOptions> mailcoachOptions)
    {
        this.httpClient = httpClient;
        this.logger = logger;
        this.mailcoachOptions = mailcoachOptions.Value;


        if (string.IsNullOrEmpty(this.mailcoachOptions.ApiDomain) ||
            string.IsNullOrEmpty(this.mailcoachOptions.ApiToken))
        {
            logger.LogWarning("MailcoachService: Mailcoach API not configured properly");
        }
        else
        {
            httpClient.BaseAddress = new Uri($"https://{this.mailcoachOptions.ApiDomain.TrimEnd('/')}/api/");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", mailcoachOptions.Value.ApiToken);
        }
    }

    /// <inheritdoc />
    public async Task<bool> AddSubscriber(MailcoachSubscriber subscriber, string emailListId)
    {
        var endpoint = $"email-lists/{emailListId}/subscribers";

        var requestBody = JsonSerializer.Serialize(subscriber);

        var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync(endpoint, content);

        if (response.IsSuccessStatusCode)
        {
            logger.LogInformation("Successfully added subscriber {email} to Mailcoach list {ListId}",
                subscriber.Email, emailListId);
            return true;
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            logger.LogError("Failed to add subscriber {email} to Mailcoach list {ListId}. Status: {StatusCode}, Response: {Response}",
                subscriber.Email, emailListId, response.StatusCode, errorContent);
            return false;
        }
    }

    /// <inheritdoc />
    public async Task<List<MailcoachEmailList>> GetMailingListsAsync()
    {
        try
        {
            var endpoint = $"email-lists";

            var response = await httpClient.GetAsync(endpoint);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var emailListsResponse = JsonSerializer.Deserialize<MailcoachEmailListsResponse>(content);

                if (emailListsResponse?.Data != null)
                {
                    logger.LogDebug("Successfully retrieved {count} email lists from Mailcoach",
                        emailListsResponse.Data.Count);
                    return emailListsResponse.Data;
                }
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                logger.LogError("Failed to retrieve email lists from Mailcoach. Status: {statusCode}, Response: {response}",
                    response.StatusCode, errorContent);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception occurred while retrieving email lists from Mailcoach");
        }

        return [];
    }
}