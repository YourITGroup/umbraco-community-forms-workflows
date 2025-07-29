using Microsoft.AspNetCore.Mvc;
using Our.Umbraco.Forms.Mailcoach.Services;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;

namespace Our.Umbraco.Forms.Mailcoach.Controllers;

[PluginController("Mailcoach")]
public class MailcoachApiController(IMailcoachService mailcoachService) : UmbracoAuthorizedApiController
{
    [HttpGet]
    public async Task<IActionResult> GetEmailLists()
    {
        try
        {
            var emailLists = await mailcoachService.GetEmailListsAsync();
            
            var result = emailLists.Select(list => new
            {
                value = list.Id,
                label = list.Name
            }).ToList();

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}