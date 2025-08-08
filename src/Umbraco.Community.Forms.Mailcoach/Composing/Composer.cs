using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Community.Forms.Mailcoach.Configuration;
using Umbraco.Community.Forms.Mailcoach.Services;
using Umbraco.Community.Forms.Mailcoach.Workflows;
using Umbraco.Forms.Core.Providers;

namespace Umbraco.Community.Forms.Mailcoach.Composing;

public class Composer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.ConfigureOptions<FormsWorkflowsApiSwaggerGenOptions>();
        
        builder.Services.Configure<MailcoachOptions>(builder.Config.GetSection(MailcoachOptions.SectionName));
        builder.Services.AddHttpClient<IMailcoachService, MailcoachService>();

        builder.WithCollectionBuilder<WorkflowCollectionBuilder>()
            .Add<MailcoachWorkflow>();
            
    }
}