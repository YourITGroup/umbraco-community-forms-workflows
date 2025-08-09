using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Umbraco.Community.Forms.Workflows.Configuration;
using Umbraco.Community.Forms.Workflows.Models.Mailcoach;
using Umbraco.Extensions;

namespace Umbraco.Community.Forms.Workflows.Services;

public class MailcoachService : IMailcoachService
{
    private readonly CommunityOptions options;
    private readonly HttpClient httpClient;
    private readonly ILogger<MailcoachService> logger;

    public MailcoachService(
        HttpClient httpClient,
        ILogger<MailcoachService> logger,
        IOptions<CommunityOptions> options)
    {
        this.httpClient = httpClient;
        this.logger = logger;
        this.options = options.Value;

        ConfigureBaseAddress();
        ConfigureApiToken();
    }

    /// <inheritdoc />
    public async Task<bool> AddSubscriber(Subscriber subscriber, string emailListId)
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
    public async Task<List<EmailList>> GetMailingListsAsync()
    {
        try
        {
            var endpoint = $"email-lists";

            var response = await httpClient.GetAsync(endpoint);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var emailListsResponse = JsonSerializer.Deserialize<EmailListsResponse>(content);

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

    /// <inheritdoc />
    public void ConfigureBaseAddress(string? domain = null)
    {
        domain ??= options.Mailcoach?.ApiDomain;
        if (domain.IsNullOrWhiteSpace())
        {
            logger.LogWarning("ConfigureBaseAddress: Mailcoach API not configured properly");
        }

        httpClient.BaseAddress = new Uri($"https://{domain?.TrimEnd(['/', ' '])}/api/");
    }

    /// <inheritdoc />
    public void ConfigureApiToken(string? token = null)
    {
        token ??= options.Mailcoach?.ApiToken;
        if (token.IsNullOrWhiteSpace())
        {
            logger.LogWarning("ConfigureApiToken: Mailcoach API not configured properly");
        }
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}