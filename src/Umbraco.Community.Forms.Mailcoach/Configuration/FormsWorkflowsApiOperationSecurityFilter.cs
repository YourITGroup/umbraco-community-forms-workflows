using Umbraco.Cms.Api.Management.OpenApi;

namespace Umbraco.Community.Forms.Mailcoach.Configuration;

public class FormsWorkflowsApiOperationSecurityFilter : BackOfficeSecurityRequirementsOperationFilterBase
{
	protected override string ApiName => Constants.ApiName;
}