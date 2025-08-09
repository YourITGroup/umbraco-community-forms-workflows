# Workflows for Umbraco Forms

![Workflows for Umbraco Forms Logo](https://raw.githubusercontent.com/YourITGroup/our-umbraco-forms-workflows/main/GithubFiles/Logo/Subscribe_logo.png)

Umbraco Forms Workflows by the community.

Workflows include:

* Mailcoach Email Subscription sign-up
* MailChimp Email Subscription sign-up

## Installation

1. Install the package via NuGet or include the project in your Umbraco solution
2. Build and run your Umbraco application
3. The workflow will be automatically registered and available in Umbraco Forms

## Configuration

### Mailcoach

Add Mailcoach Settings to `appsettings.json` with the following configuration:

```json
{
  "Mailcoach": {
    "ApiDomain": "your-mailcoach-domain.com",
    "ApiToken": "your-mailcoach-api-token"
  }
}
```


## Requirements

- Umbraco CMS 16+
- Umbraco Forms 16+
- .NET 9.0

## Logo

The package logo uses the "mailing list" (by Thomas Deckert) icon from the Noun Project, licensed under CC BY 3.0 US.
 