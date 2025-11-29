using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Umbraco.Community.Forms.Workflows.Configuration;

public class FormsWorkflowsApiSwaggerGenOptions : IConfigureOptions<SwaggerGenOptions>
{
	public void Configure(SwaggerGenOptions options)
	{
		options.SwaggerDoc(
		  Constants.ApiName,
		  new OpenApiInfo
		  {
			  Title = "Umbraco Community Forms Workflow Management Api",
			  Version = "Latest",
			  Description = "Api access for Umbraco Community Forms Workflow Management operations"
		  });

		options.OperationFilter<FormsWorkflowsApiOperationSecurityFilter>();
	}
}