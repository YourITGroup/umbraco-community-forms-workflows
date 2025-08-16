using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Umbraco.Community.Forms.Workflows.Configuration;
using Umbraco.Extensions;
using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Providers.Models;
using Umbraco.Forms.Core.Services;
using MailChimp.Net;
using MailChimp.Net.Models;
using Umbraco.Community.Forms.Workflows.Models.MailChimp;

namespace Umbraco.Community.Forms.Workflows.Workflows;

public class MailChimpWorkflow : WorkflowType
{
    private readonly IPlaceholderParsingService placeholderParsingService;
    private readonly ILogger<MailChimpWorkflow> logger;
    private readonly CommunityOptions options;

    [Setting("Email List",
        Description = "Configure the MailChimp email list to add subscribers to.  API token is required if not already set globally",
        View = "Umbraco.Forms.Community.Workflows.PropertyEditorUi.MailChimpMailingList")]
    public string EmailListConfiguration { get; set; } = string.Empty;

    [Setting("Email",
        Description = "Select the field to map the subscriber email address to",
        View = "Forms.PropertyEditorUi.TextWithFieldPicker")]
    public string Email { get; set; } = string.Empty;

    [Setting("Fields",
        Description = "Map form fields to MailChimp subscriber attributes.",
        View = "Forms.PropertyEditorUi.FieldMapper")]
    public string Fields { get; set; } = string.Empty;

    [Setting("Tags",
        Description = "List of Tags. Tag must be created before being used.",
        View = "Umb.PropertyEditorUi.MultipleTextString")]
    public string Tags { get; set; } = string.Empty;

    [Setting("Set Pending Status",
        Description = "Enable the 'Double Opt In' feature",
        View = "Umb.PropertyEditorUi.Toggle")]
    public string MarkAsPending { get; set; } = "false";

    public MailChimpWorkflow(IPlaceholderParsingService placeholderParsingService, ILogger<MailChimpWorkflow> logger, IOptions<CommunityOptions> options)
    {
        this.placeholderParsingService = placeholderParsingService;
        this.logger = logger;
        this.options = options.Value;

        Id = new Guid("0deb3da1-422f-42ba-8b63-741a2be82f9c");
        Name = "MailChimp Subscriber";
        Description = "Add form submitter to a MailChimp email list";
        Icon = "icon-mailbox";
        Group = "Services";
    }

    public override List<Exception> ValidateSettings()
    {
        var exceptions = new List<Exception>();

        if (string.IsNullOrEmpty(EmailListConfiguration))
        {
            exceptions.Add(new ArgumentException("Email List is not configured"));
        }

        if (string.IsNullOrEmpty(Email))
        {
            exceptions.Add(new ArgumentException("Email field is not configured"));
        }

        return exceptions;
    }

    public override async Task<WorkflowExecutionStatus> ExecuteAsync(WorkflowExecutionContext context)
    {
        try
        {
            var listConfig = JsonSerializer.Deserialize<MailingListConfig>(EmailListConfiguration);
            if (listConfig is null || listConfig.ListId is null)
            {
                logger.LogWarning("MailChimp workflow not configured properly - missing Mailing List configuration");
                return WorkflowExecutionStatus.NotConfigured;
            }

            List<FieldMapping> mappings = [];

            if (!string.IsNullOrEmpty(Fields))
            {
                var source = JsonSerializer.Deserialize<IEnumerable<FieldMapping>>(Fields, FormsJsonSerializerOptions.Default);
                if (source != null)
                    mappings = [.. source.Select(x =>
                        {
                            x.StaticValue = placeholderParsingService.ParsePlaceHolders(x.StaticValue, false, context.Record);
                            return x;
                        })];
            }
            var email = placeholderParsingService.ParsePlaceHolders(Email, false, context.Record);

            if (string.IsNullOrEmpty(email))
            {
                logger.LogWarning("No email address found in form submission");
                return WorkflowExecutionStatus.Failed;
            }

            var mc = new MailChimpManager(listConfig.ApiKey ?? options.MailChimp.ApiKey);
            await SubscribeMember(mc, listConfig.ListId, email, mappings);
            if (!string.IsNullOrEmpty(Tags))
            {
                var tags = Tags.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim());
                await TagMember(mc, listConfig.ListId, email, tags);
            }
            return WorkflowExecutionStatus.Completed;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error executing MailChimp workflow");
            return WorkflowExecutionStatus.Failed;
        }
    }

    private async Task SubscribeMember(MailChimpManager mc, string listId, string email, List<FieldMapping> mergeFields)
    {
        _ = bool.TryParse(MarkAsPending, out bool skipConfirmation);

        var member = new Member
        {
            EmailAddress = email,
            Status = Status.Subscribed,
            EmailType = "html",
            TimestampOpt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            StatusIfNew = skipConfirmation ? Status.Pending : Status.Subscribed,
            MergeFields = mergeFields.ToDictionary(k => k.Alias, v => (object)v.Value)
        };

        await mc.Members.AddOrUpdateAsync(listId, member);
    }
    
    private async Task TagMember(MailChimpManager mc, string listId, string email, IEnumerable<string> tagNames)
    {
        var tagIds = await GetExistingTagIds(listId, tagNames);
        var member = new Member { EmailAddress = email };

        foreach (int tagId in tagIds)
        {
            await mc.ListSegments.AddMemberAsync(listId, $"{tagId}", member);
        }
    }
        
    private async Task<IEnumerable<int>> GetExistingTagIds(string listId, IEnumerable<string> tagNames)
    {
        var mc = new MailChimpManager(options.MailChimp.ApiKey);

        var tagsMap = (await mc.ListSegments.GetAllAsync(listId))
            .Where(s => s.Type == "static")
            .Aggregate(
                new Dictionary<string, int>(),
                (dict, seg) =>
                {
                    dict.Add(seg.Name, seg.Id);
                    return dict;
                }
            );

        var tagIds = tagNames
            .Where(tagsMap.ContainsKey)
            .Select(t => tagsMap[t]);

        return tagIds;
    }
}