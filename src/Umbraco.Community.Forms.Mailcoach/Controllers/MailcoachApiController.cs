using Microsoft.AspNetCore.Mvc;
using Umbraco.Community.Forms.Mailcoach.Services;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;

namespace Umbraco.Community.Forms.Mailcoach.Controllers;

[PluginController("Mailcoach")]
public class MailcoachApiController(IMailcoachService mailcoachService) : UmbracoAuthorizedApiController
{
    [HttpGet]
    public async Task<IActionResult> GetMailingLists()
    {
        try
        {
            var emailLists = await mailcoachService.GetMailingListsAsync();

            var result = emailLists;
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}