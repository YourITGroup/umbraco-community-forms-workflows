using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Api.Management.Controllers;
using Umbraco.Community.Forms.Mailcoach.Models;
using Umbraco.Community.Forms.Mailcoach.Services;

namespace Umbraco.Community.Forms.Mailcoach.Controllers;

[ApiExplorerSettings(GroupName = "Config")]
[ApiVersion("1.0")]
[Authorize(Policy = "SectionAccessForms")]
[MapToApi(Constants.ApiName)]
public class MailcoachController(IMailcoachService mailcoachService) : ManagementApiControllerBase
{
    [HttpGet("MailingLists")]
    [ProducesResponseType(typeof(IEnumerable<MailcoachEmailList>), 200)]
    public async Task<IEnumerable<MailcoachEmailList>> GetMailingLists()
    {
        return await mailcoachService.GetMailingListsAsync();
    }
}