using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Community.Forms.Workflows.Configuration;
using Umbraco.Community.Forms.Workflows.Services;
using Umbraco.Community.Forms.Workflows.Workflows;
using Umbraco.Forms.Core.Providers;

namespace Umbraco.Community.Forms.Workflows.Composing;

public class Composer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.ConfigureOptions<FormsWorkflowsApiSwaggerGenOptions>();
        
        builder.Services.Configure<CommunityOptions>(builder.Config.GetSection(CommunityOptions.SectionName));
        builder.Services.AddHttpClient<IMailcoachService, MailcoachService>();

        builder.WithCollectionBuilder<WorkflowCollectionBuilder>()
            .Add<CampaignMonitorWorkflow>()
            .Add<MailcoachWorkflow>()
            .Add<MailChimpWorkflow>();
            
    }
}