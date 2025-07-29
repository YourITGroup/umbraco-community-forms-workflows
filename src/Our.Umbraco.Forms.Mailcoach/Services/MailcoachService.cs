using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Our.Umbraco.Forms.Mailcoach.Configuration;
using Our.Umbraco.Forms.Mailcoach.Models;

namespace Our.Umbraco.Forms.Mailcoach.Services;

public class MailcoachService(
    IHttpClientFactory httpClientFactory,
    ILogger<MailcoachService> logger,
    IOptions<MailcoachOptions> mailcoachOptions) : IMailcoachService
{
    private readonly MailcoachOptions _mailcoachOptions = mailcoachOptions.Value;

    public async Task<List<MailcoachEmailList>> GetEmailListsAsync()
    {
        try
        {
            if (string.IsNullOrEmpty(_mailcoachOptions.ApiEndpoint) || 
                string.IsNullOrEmpty(_mailcoachOptions.ApiToken))
            {
                logger.LogWarning("Mailcoach API not configured properly");
                return new List<MailcoachEmailList>();
            }

            using var httpClient = httpClientFactory.CreateClient();
            
            var endpoint = $"{_mailcoachOptions.ApiEndpoint.TrimEnd('/')}/api/email-lists";
            
            httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _mailcoachOptions.ApiToken);

            var response = await httpClient.GetAsync(endpoint);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var emailListsResponse = JsonConvert.DeserializeObject<MailcoachEmailListsResponse>(content);
                
                if (emailListsResponse?.Data != null)
                {
                    logger.LogDebug("Successfully retrieved {Count} email lists from Mailcoach", 
                        emailListsResponse.Data.Count);
                    return emailListsResponse.Data;
                }
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                logger.LogError("Failed to retrieve email lists from Mailcoach. Status: {StatusCode}, Response: {Response}", 
                    response.StatusCode, errorContent);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception occurred while retrieving email lists from Mailcoach");
        }

        return new List<MailcoachEmailList>();
    }
}