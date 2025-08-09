using Asp.Versioning;
using MailChimp.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Api.Management.Controllers;
using Umbraco.Community.Forms.Workflows.Configuration; 

namespace Umbraco.Community.Forms.Workflows.Controllers;

[ApiExplorerSettings(GroupName = "Config")]
[ApiVersion("1.0")]
[Authorize(Policy = "SectionAccessForms")]
[MapToApi(Constants.ApiName)]
public class MailChimpController(IOptions<CommunityOptions> options) : ManagementApiControllerBase
{
    [HttpGet("MailChimpLists")]
    [ProducesResponseType(typeof(IEnumerable<MailChimp.Net.Models.List>), 200)]
    public async Task<IEnumerable<MailChimp.Net.Models.List>> GetMailChimpLists(string? apiKey)
    {
        apiKey ??= options.Value.MailChimp.ApiKey;
        try
        {
            var mailChimp = new MailChimpManager(apiKey);
            return await mailChimp.Lists.GetAllAsync();
        }
        catch
        {
            return [];
        }
    }
}