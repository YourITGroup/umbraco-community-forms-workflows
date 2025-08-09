import type { ManifestGlobalContext } from '@umbraco-cms/backoffice/extension-registry';

const contexts: Array<ManifestGlobalContext> = [
  {
    type: 'globalContext',
    alias: 'Umbraco.Forms.Community.Mailcoach.Config.Context',
    name: 'Umbraco Forms Community Mailcoach Config Context',
    api: () => import('./mailcoach.config.context.js'),
  },
  {
    type: 'globalContext',
    alias: 'Umbraco.Forms.Community.MailChimp.Config.Context',
    name: 'Umbraco Forms Community MailChimp Config Context',
    api: () => import('./mailchimp.config.context.js'),
  },
];

export const manifests = contexts;