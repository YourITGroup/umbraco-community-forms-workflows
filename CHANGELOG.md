# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2024-07-29

### Added
- Initial implementation of Mailcoach workflow for Umbraco Forms
- Support for adding form subscribers to Mailcoach email lists
- Configuration-based API settings via appsettings.json for security
- Native Umbraco Forms workflow UI using `[Setting]` attributes:
  - Custom email list picker that loads available lists from Mailcoach API
  - FieldMapper for flexible field mapping (similar to Mailchimp workflow)
  - Support for form fields and static values
  - Direct Tags setting for applying tags to all subscribers
  - Option to skip email confirmation
  - Support for custom attributes via `extra_attributes`
- Support for Umbraco Forms 13.5+ and .NET 8
- Comprehensive error logging and handling
- Package targeting Umbraco CMS with Mailcoach integration

### Security
- API credentials stored in configuration instead of workflow settings
- Secure handling of API tokens via dependency injection

### Implementation
- Uses modern C# patterns with collection expressions
- Integrated with Umbraco's dependency injection container
- Follows Umbraco Forms workflow best practices

### Dependencies
- Umbraco.Forms.Core 13.5.0
- .NET 8.0 target framework