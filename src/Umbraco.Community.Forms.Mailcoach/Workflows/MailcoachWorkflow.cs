using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Umbraco.Community.Forms.Mailcoach.Configuration;
using Umbraco.Community.Forms.Mailcoach.Models;
using Umbraco.Community.Forms.Mailcoach.Services;
using Umbraco.Extensions;
using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Providers.Models;
using Umbraco.Forms.Core.Persistence.Dtos;
using Umbraco.Forms.Core.Services;

namespace Umbraco.Community.Forms.Mailcoach.Workflows;

public class MailcoachWorkflow : WorkflowType
{
    private readonly IPlaceholderParsingService placeholderParsingService;
    private readonly IMailcoachService mailcoachService;
    private readonly ILogger<MailcoachWorkflow> _logger;
    private readonly MailcoachOptions _mailcoachOptions;

    [Setting("Email List",
        Description = "Select the Mailcoach email list to add subscribers to",
        View = "~/App_Plugins/Mailcoach/MailcoachEmailListPicker.html")]
    public string EmailListId { get; set; } = string.Empty;

    [Setting("Fields",
        Description = "Map form fields to Mailcoach subscriber attributes.",
        View = "FieldMapper")]
    public string Fields { get; set; } = string.Empty;

    [Setting("Email",
        Description = "Select the field to map the subscriber email address to",
        View = "TextWithFieldPicker")]
    public string Email { get; set; } = string.Empty;

    [Setting("Tags",
        Description = "Comma-separated tags to apply to all subscribers (optional)",
        View = "TextField")]
    public string Tags { get; set; } = string.Empty;

    [Setting("Skip Confirmation",
        Description = "Subscribe users immediately without confirmation email",
        View = "Checkbox")]
    public string SkipConfirmation { get; set; } = "false";

    public MailcoachWorkflow(IPlaceholderParsingService placeholderParsingService, IMailcoachService mailcoachService, ILogger<MailcoachWorkflow> logger, IOptions<MailcoachOptions> mailcoachOptions)
    {
        this.placeholderParsingService = placeholderParsingService;
        this.mailcoachService = mailcoachService;
        _logger = logger;
        _mailcoachOptions = mailcoachOptions.Value;

        Id = new Guid("A4D5B1E8-2C3F-4A5B-8D9E-1F2A3B4C5D6E");
        Name = "Mailcoach Subscriber";
        Description = "Add form submitter to a Mailcoach email list";
        Icon = "icon-mailbox";
        Group = "Services";
    }

    public override List<Exception> ValidateSettings()
    {
        var exceptions = new List<Exception>();

        if (string.IsNullOrEmpty(_mailcoachOptions.ApiDomain))
        {
            exceptions.Add(new ArgumentException("Mailcoach API Endpoint is not configured in appsettings.json"));
        }

        if (string.IsNullOrEmpty(_mailcoachOptions.ApiToken))
        {
            exceptions.Add(new ArgumentException("Mailcoach API Token is not configured in appsettings.json"));
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
            if (string.IsNullOrEmpty(_mailcoachOptions.ApiDomain) ||
                string.IsNullOrEmpty(_mailcoachOptions.ApiToken) ||
                string.IsNullOrEmpty(EmailListId))
            {
                _logger.LogWarning("Mailcoach workflow not configured properly");
                return WorkflowExecutionStatus.NotConfigured;
            }

            var subscriber = MapFormDataToSubscriber(context);

            if (string.IsNullOrEmpty(subscriber.Email))
            {
                _logger.LogWarning("No email address found in form submission");
                return WorkflowExecutionStatus.Failed;
            }

            var success = await mailcoachService.AddSubscriber(subscriber, EmailListId);

            return success ? WorkflowExecutionStatus.Completed : WorkflowExecutionStatus.Failed;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing Mailcoach workflow");
            return WorkflowExecutionStatus.Failed;
        }
    }

    private MailcoachSubscriber MapFormDataToSubscriber(WorkflowExecutionContext context)
    {
        var subscriber = new MailcoachSubscriber();

        List<FieldMapping> mappings = [];
        if (!string.IsNullOrEmpty(Fields))
        {
            var source = JsonConvert.DeserializeObject<IEnumerable<FieldMapping>>(Fields, FormsJsonSerializerSettings.Default) ?? [];
            if (source != null)
            {
                mappings = [.. source.Select(x =>
                {
                    x.StaticValue = placeholderParsingService.ParsePlaceHolders(x.StaticValue, false, context.Record);
                    return x;
                })];
            }
        }

        try
        {
            subscriber.Email = GetTextWithFieldPicker(Email, context.Record) ?? string.Empty;
            foreach (var mapping in mappings)
            {
                var fieldValue = GetMappedFieldValue(mapping, context.Record);
                if (string.IsNullOrEmpty(fieldValue)) continue;

                switch (mapping.Alias.ToLowerInvariant())
                {
                    case "first_name":
                    case "firstname":
                        subscriber.FirstName = fieldValue;
                        break;
                    case "last_name":
                    case "lastname":
                        subscriber.LastName = fieldValue;
                        break;
                    default:
                        // For any other attributes, add to extra_attributes
                        subscriber.ExtraAttributes ??= [];
                        subscriber.ExtraAttributes[mapping.Alias] = fieldValue;
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing field mappings configuration");
        }

        // Apply tags from the direct Tags setting
        if (!string.IsNullOrEmpty(Tags))
        {
            subscriber.Tags = [.. Tags.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.Trim())];
        }
        if (bool.TryParse(SkipConfirmation, out bool skipConfirmation))
        {
            subscriber.SkipConfirmation = skipConfirmation;
        }
        return subscriber;
    }

    private string GetMappedFieldValue(FieldMapping mapping, Record record)
    {
        if (!string.IsNullOrEmpty(mapping.StaticValue))
        {
            return mapping.StaticValue;
        }
        else if (!string.IsNullOrEmpty(mapping.Value))
        {
            var recordField = record.GetRecordField(new Guid(mapping.Value));
            if (recordField != null)
            {
                return recordField.ValuesAsString(false);
            }
            else
                _logger.LogWarning("Workflow {WorkflowName}: The field mapping with alias, {FieldMappingAlias}, did not match any record fields. This is probably caused by the record field being marked as sensitive and the workflow has been set not to include sensitive data", (object)this.Workflow?.Name, (object)mapping.Alias);
        }


        return string.Empty;
    }

    private string GetTextWithFieldPicker(string field, Record record)
    {
        if (field.StartsWith('{') && field.EndsWith('}'))
        {
            var alias = field.Trim('{', '}');
            var recordField = record.RecordFields.FirstOrDefault(f => f.Value.Alias.InvariantEquals(alias)).Value;
            return recordField.ValuesAsString();
        }
        else
        {
            return field;
        }
    }
}