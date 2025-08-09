using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Api.Management.Controllers;
using Umbraco.Community.Forms.Workflows.Models;
using Umbraco.Community.Forms.Workflows.Models.Mailcoach;
using Umbraco.Community.Forms.Workflows.Services;

namespace Umbraco.Community.Forms.Workflows.Controllers;

[ApiExplorerSettings(GroupName = "Config")]
[ApiVersion("1.0")]
[Authorize(Policy = "SectionAccessForms")]
[MapToApi(Constants.ApiName)]
public class MailcoachController(IMailcoachService mailcoachService) : ManagementApiControllerBase
{
    [HttpGet("MailCoachLists")]
    [ProducesResponseType(typeof(IEnumerable<EmailList>), 200)]
    public async Task<IEnumerable<EmailList>> GetMailCoachLists(string? domain, string? token)
    {
        mailcoachService.ConfigureBaseAddress(domain);
        mailcoachService.ConfigureApiToken(token);
        try
        {
            return await mailcoachService.GetMailingListsAsync();
        }
        catch
        {
            return [];
        }
    }
}