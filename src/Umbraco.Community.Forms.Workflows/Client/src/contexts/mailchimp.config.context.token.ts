import type { MailChimpConfigContext } from "./mailchimp.config.context.js";
import { UmbContextToken } from "@umbraco-cms/backoffice/context-api";

export const MAILCHIMP_CONFIG_CONTEXT = new UmbContextToken<MailChimpConfigContext>("mailchimp-config-context");