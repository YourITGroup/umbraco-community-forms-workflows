using Microsoft.Extensions.DependencyInjection;
using Umbraco.Community.Forms.Mailcoach.Configuration;
using Umbraco.Community.Forms.Mailcoach.Services;
using Umbraco.Community.Forms.Mailcoach.Workflows;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Forms.Core.Providers;

namespace Umbraco.Community.Forms.Mailcoach.Composers;

public class MailcoachWorkflowComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.Configure<MailcoachOptions>(builder.Config.GetSection(MailcoachOptions.SectionName));
        builder.Services.AddHttpClient<IMailcoachService, MailcoachService>();
        
        builder.WithCollectionBuilder<WorkflowCollectionBuilder>()
            .Add<MailcoachWorkflow>();
    }
}