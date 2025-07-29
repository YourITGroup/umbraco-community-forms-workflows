# Mailcoach Subscription workflow for Umbraco Forms

![Mailcoach for Umbraco Forms Logo](https://raw.githubusercontent.com/YourITGroup/Our.Umbraco.Forms.Mailcoach/master/GithubFiles/Logo/Subscribe_logo.png)

An Umbraco Forms workflow that automatically adds form subscribers to Mailcoach email lists.

## Features

- Seamless integration between Umbraco Forms and Mailcoach
- **Custom email list picker** - Select from your Mailcoach email lists via dropdown
- **FieldMapper integration** - Visual mapping of form fields to subscriber attributes
- Support for email, first name, last name, and custom attributes
- Direct tags configuration for all subscribers
- Option to skip email confirmation for immediate subscription
- Comprehensive error logging and handling

## Configuration

### 1. Add Mailcoach Settings to appsettings.json

Add the following configuration to your `appsettings.json`:

```json
{
  "Mailcoach": {
    "ApiEndpoint": "https://your-mailcoach-domain.com",
    "ApiToken": "your-mailcoach-api-token"
  }
}
```

### 2. Configure the Workflow in Umbraco

1. In your Umbraco backoffice, go to Forms and edit a form
2. Navigate to the Workflows section
3. Add the "Mailcoach Subscriber" workflow
4. Configure the workflow settings:
   - **Email List**: Select from your available Mailcoach email lists (automatically loaded from your Mailcoach account)
   - **Fields**: Use the FieldMapper to map form fields to Mailcoach subscriber attributes:
     - **email** (required): Map to the form field containing the email address
     - **first_name** (optional): Map to the form field containing the first name
     - **last_name** (optional): Map to the form field containing the last name
     - **custom fields**: Any other field alias will be added to `extra_attributes`
   - **Tags**: Comma-separated tags to apply to all subscribers (optional)
   - **Skip Confirmation**: Check to subscribe users immediately without confirmation email

### Field Mapping Options

The FieldMapper provides flexible configuration:
- **Form Fields**: Select any form field from dropdowns
- **Static Values**: Use static text values instead of form fields
- **Custom Attributes**: Map to any Mailcoach subscriber attribute using the alias

**Standard Mailcoach Fields:**
- `email` - Email address (required)
- `first_name` - First name
- `last_name` - Last name

Any other alias will be stored in the subscriber's `extra_attributes`.

**Tags Configuration:**
Tags are configured as a direct setting (not mapped from form fields) and applied to all subscribers. Use comma-separated values like: `newsletter,signup,website`

## Requirements

- Umbraco CMS 13+
- Umbraco Forms 13.5+
- .NET 8.0
- Valid Mailcoach account with API access
