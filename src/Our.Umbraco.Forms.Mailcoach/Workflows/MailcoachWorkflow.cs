using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Our.Umbraco.Forms.Mailcoach.Configuration;
using Our.Umbraco.Forms.Mailcoach.Models;
using Our.Umbraco.Forms.Mailcoach.Services;
using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Enums;

namespace Our.Umbraco.Forms.Mailcoach.Workflows;

public class MailcoachWorkflow : WorkflowType
{
    private readonly IMailcoachService mailcoachService;
    private readonly ILogger<MailcoachWorkflow> _logger;
    private readonly MailcoachOptions _mailcoachOptions;

    [Setting("Email List",
        Description = "Select the Mailcoach email list to add subscribers to",
        View = "~/App_Plugins/Mailcoach/MailcoachEmailListPicker.html")]
    public string EmailListId { get; set; } = string.Empty;

    [Setting("Fields",
        Description = "Map form fields to Mailcoach subscriber attributes. Email field is required.",
        View = "FieldMapper")]
    public string Fields { get; set; } = string.Empty;

    [Setting("Tags",
        Description = "Comma-separated tags to apply to all subscribers (optional)",
        View = "TextField")]
    public string Tags { get; set; } = string.Empty;

    [Setting("Skip Confirmation",
        Description = "Subscribe users immediately without confirmation email",
        View = "Checkbox")]
    public bool SkipConfirmation { get; set; } = false;

    public MailcoachWorkflow(IMailcoachService mailcoachService, ILogger<MailcoachWorkflow> logger, IOptions<MailcoachOptions> mailcoachOptions)
    {
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
                return WorkflowExecutionStatus.Failed;
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
        
        if (string.IsNullOrEmpty(Fields))
        {
            _logger.LogWarning("No field mappings configured for Mailcoach workflow");
            return subscriber;
        }

        try
        {
            var fieldMappings = JsonConvert.DeserializeObject<List<FieldMapping>>(Fields);
            if (fieldMappings == null || !fieldMappings.Any())
            {
                _logger.LogWarning("Invalid or empty field mappings configuration");
                return subscriber;
            }

            foreach (var mapping in fieldMappings)
            {
                var fieldValue = GetMappedFieldValue(context, mapping);
                if (string.IsNullOrEmpty(fieldValue)) continue;

                switch (mapping.Alias.ToLowerInvariant())
                {
                    case "email":
                        subscriber.Email = fieldValue;
                        break;
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
                        subscriber.ExtraAttributes ??= new Dictionary<string, object>();
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

        subscriber.SkipConfirmation = SkipConfirmation;
        return subscriber;
    }

    private static string GetMappedFieldValue(WorkflowExecutionContext context, FieldMapping mapping)
    {
        // If static value is provided, use it
        if (!string.IsNullOrEmpty(mapping.StaticValue))
        {
            return mapping.StaticValue;
        }

        // Try to parse the field ID as a GUID
        if (Guid.TryParse(mapping.Value, out var fieldId))
        {
            if (context.Record.RecordFields.TryGetValue(fieldId, out var recordField))
            {
                return recordField.ValuesAsString();
            }
        }

        return string.Empty;
    }
}