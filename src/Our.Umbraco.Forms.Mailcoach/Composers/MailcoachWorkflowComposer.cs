using Microsoft.Extensions.DependencyInjection;
using Our.Umbraco.Forms.Mailcoach.Configuration;
using Our.Umbraco.Forms.Mailcoach.Services;
using Our.Umbraco.Forms.Mailcoach.Workflows;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Forms.Core.Providers;

namespace Our.Umbraco.Forms.Mailcoach.Composers;

public class MailcoachWorkflowComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.Configure<MailcoachOptions>(builder.Config.GetSection(MailcoachOptions.SectionName));
        builder.Services.AddScoped<IMailcoachService, MailcoachService>();
        
        builder.WithCollectionBuilder<WorkflowCollectionBuilder>()
            .Add<MailcoachWorkflow>();
    }
}