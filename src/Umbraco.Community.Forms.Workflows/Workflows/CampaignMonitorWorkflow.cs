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
using Umbraco.Community.Forms.Workflows.Models.CampaignMonitor;
using createsend_dotnet;

namespace Umbraco.Community.Forms.Workflows.Workflows;

public class CampaignMonitorWorkflow : WorkflowType
{
    private readonly IPlaceholderParsingService placeholderParsingService;
    private readonly ILogger<CampaignMonitorWorkflow> logger;
    private readonly CommunityOptions options;

    [Setting("Email List",
        Description = "Configure the CampaignMonitor email list to add subscribers to.  API token is required if not already set globally",
        View = "Umbraco.Forms.Community.Workflows.PropertyEditorUi.CampaignMonitorMailingList")]
    public string EmailListConfiguration { get; set; } = string.Empty;

    [Setting("Email",
        Description = "Select the field to map the subscriber email address to",
        View = "Forms.PropertyEditorUi.TextWithFieldPicker")]
    public string Email { get; set; } = string.Empty;

    [Setting("Name",
        Description = "Select the field to map the subscriber name to",
        View = "Forms.PropertyEditorUi.TextWithFieldPicker")]
    public string SubscriberName { get; set; } = string.Empty;

    [Setting("Custom Fields",
        Description = "Map form fields to CampaignMonitor custom subscriber fields.",
        View = "Forms.PropertyEditorUi.FieldMapper")]
    public string CustomFields { get; set; } = string.Empty;

    [Setting("Consent To Track",
        Description = "Select a true/false to opt in to tracking",
        View = "Forms.PropertyEditorUi.TextWithFieldPicker")]
    public string ConsentToTrack { get; set; } = "false";

    [Setting("Consent To Send SMS",
        Description = "Select a true/false field to opt in to SMS sending",
        View = "Forms.PropertyEditorUi.TextWithFieldPicker")]
    public string ConsentToSendSMS { get; set; } = "false";

    [Setting("Mobile",
        Description = "Select the field to map the subscriber mobile number to for SMS sending",
        View = "Forms.PropertyEditorUi.TextWithFieldPicker")]
    public string Mobile { get; set; } = string.Empty;

    public CampaignMonitorWorkflow(IPlaceholderParsingService placeholderParsingService, ILogger<CampaignMonitorWorkflow> logger, IOptions<CommunityOptions> options)
    {
        this.placeholderParsingService = placeholderParsingService;
        this.logger = logger;
        this.options = options.Value;

        Id = new Guid("d3f8c9a4-7b2e-4f5a-9c3e-8a1b2c4d5e6f");
        Name = "CampaignMonitor Subscriber";
        Description = "Add form submitter to a CampaignMonitor email list";
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
                logger.LogWarning("CampaignMonitor workflow not configured properly - missing Mailing List configuration");
                return WorkflowExecutionStatus.NotConfigured;
            }

            List<SubscriberCustomField> customFields = [];

            if (!string.IsNullOrEmpty(CustomFields))
            {
                var source = JsonSerializer.Deserialize<IEnumerable<FieldMapping>>(CustomFields, FormsJsonSerializerOptions.Default);
                if (source != null)
                    customFields = [.. source.Select(x =>
                        {
                            return new SubscriberCustomField {
                                Key = x.Alias,
                                Value = GetMappedFieldValue(x, context)
                            };
                        })];
            }
            var email = placeholderParsingService.ParsePlaceHolders(Email, false, context.Record);
            var name = placeholderParsingService.ParsePlaceHolders(SubscriberName, false, context.Record);
            var mobile = placeholderParsingService.ParsePlaceHolders(Mobile, false, context.Record);

            var track = createsend_dotnet.ConsentToTrack.Unchanged;
            if (bool.TryParse(ConsentToTrack, out var consentToTrack))
            {
                track = consentToTrack ? createsend_dotnet.ConsentToTrack.Yes : createsend_dotnet.ConsentToTrack.No;
            }
            var sendSms = ConsentToSendSms.Unchanged;
            if (bool.TryParse(ConsentToSendSMS, out var consentToSendSMS))
            {
                sendSms = consentToSendSMS ? ConsentToSendSms.Yes : ConsentToSendSms.No;
            }

            if (string.IsNullOrEmpty(email))
            {
                logger.LogWarning("No email address found in form submission");
                return WorkflowExecutionStatus.Failed;
            }

            AuthenticationDetails auth = new ApiKeyAuthenticationDetails(listConfig.ApiKey);
            Subscriber subscriber = new(auth, listConfig.ListId);
            subscriber.Add(email, name, customFields, true, track, mobileNumber: mobile, sendSms);

            return WorkflowExecutionStatus.Completed;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error executing CampaignMonitor workflow");
            return WorkflowExecutionStatus.Failed;
        }
    }

    private string GetMappedFieldValue(FieldMapping mapping, WorkflowExecutionContext context)
    {
        if (!string.IsNullOrEmpty(mapping.StaticValue))
        {
            return mapping.StaticValue;
        }
        else if (!string.IsNullOrEmpty(mapping.Value))
        {
            var recordField = context.Record.GetRecordField(new Guid(mapping.Value));
            if (recordField != null)
            {
                return recordField.ValuesAsString(false);
            }
            else
                logger.LogWarning("Workflow {WorkflowName}: The field mapping with alias, {FieldMappingAlias}, did not match any record fields. This is probably caused by the record field being marked as sensitive and the workflow has been set not to include sensitive data", Workflow?.Name, mapping.Alias);
        }


        return string.Empty;
    }

}