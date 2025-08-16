using Asp.Versioning;
using createsend_dotnet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Api.Management.Controllers;
using Umbraco.Community.Forms.Workflows.Configuration;
using Umbraco.Extensions;

namespace Umbraco.Community.Forms.Workflows.Controllers;

[ApiExplorerSettings(GroupName = "Config")]
[ApiVersion("1.0")]
[Authorize(Policy = "SectionAccessForms")]
[MapToApi(Constants.ApiName)]
public class CampaignMonitorController(IOptions<CommunityOptions> options, ILogger<CampaignMonitorController> logger) : ManagementApiControllerBase
{
    [HttpGet("CampaignMonitorLists")]
    [ProducesResponseType(typeof(IEnumerable<BasicList>), 200)]
    public IEnumerable<BasicList> GetCampaignMonitorLists(string? apiKey, string? clientId)
    {
        apiKey ??= options.Value.CampaignMonitor?.ApiKey;
        clientId ??= options.Value.CampaignMonitor?.ClientId;
        if (apiKey.IsNullOrWhiteSpace() || clientId.IsNullOrWhiteSpace())
        {
            return [];
        }
        try
        {
            AuthenticationDetails auth = new ApiKeyAuthenticationDetails(apiKey);
            var cl = new Client(auth, clientId);
            return cl.Lists();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving lists");
            return [];
        }
    }

    [HttpGet("CampaignMonitorClients")]
    [ProducesResponseType(typeof(IEnumerable<BasicClient>), 200)]
    public IEnumerable<BasicClient> GetCampaignMonitorClients(string? apiKey)
    {
        apiKey ??= options.Value.CampaignMonitor?.ApiKey;
        if (apiKey.IsNullOrWhiteSpace())
        {
            return [];
        }
        try
        {
            AuthenticationDetails auth = new ApiKeyAuthenticationDetails(apiKey);
            var general = new General(auth);
            return general.Clients();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving clients");
            return [];
        }
    }
}