# Workflows

![Workflows for Umbraco Forms Logo](https://raw.githubusercontent.com/YourITGroup/umbraco-community-forms-workflows/main/GithubFiles/Logo/Subscribe_logo.png)

Umbraco Forms Workflows by the community for Umbraco 16.

Available workflows include:

* Mailcoach Email Subscription sign-up
* MailChimp Email Subscription sign-up
* Campaign Manager Email Subscription sign-up

## Configuration

Add Workflow Settings to `appsettings.json` with the following configuration.  The Mailing List settings are optional and can be configured directly on the Workflow:

```json
{
  "Community": {
    "Forms": {
      "Mailcoach": {
        "ApiDomain": "your-mailcoach-domain.com",
        "ApiToken": "your-mailcoach-api-token"
      },
      "MailChimp": {
        "ApiKey": "mailchimp-api-key"
      }
    }
  }
}
```

### Mailcoach Workflow

The Mailcoach workflow configuration accepts a domain name for a mailcoach server - for example, `{your-account}.mailcoach.app` or a private mailcoach server - as well as a Mailcoach token.  
If not set, these will fall back to the settings in `appsettings`.

### MailChimp Workflow

The Mailcoach workflow configuration accepts an API Key.  If not set, it will fall back to the settings in `appsettings`.


### Campaign Monitor Workflow

The Campaign Monitor workflow configuration accepts an API Key and Client (if more than one found).  If not set, it will fall back to the settings in `appsettings`.

It also supports opting in for SMS Sending and Tracking.

## Logo

The package logo uses the "mailing list" (by Thomas Deckert) icon from the Noun Project, licensed under CC BY 3.0 US.
 