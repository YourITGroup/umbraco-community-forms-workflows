import type { MailcoachConfigContext } from "./mailcoach.config.context.js";
import { UmbContextToken } from "@umbraco-cms/backoffice/context-api";

export const MAILCOACH_CONFIG_CONTEXT = new UmbContextToken<MailcoachConfigContext>("mailcoach-config-context");