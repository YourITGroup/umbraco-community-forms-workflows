import type { ManifestGlobalContext } from '@umbraco-cms/backoffice/extension-registry';

const contexts: Array<ManifestGlobalContext> = [
  {
    type: 'globalContext',
    alias: 'Umbraco.Forms.Community.Mailcoach.ConfigContext',
    name: 'Umbraco Forms Community Mailcoach Config Context',
    api: () => import('./config.context.js'),
  },
];

export const manifests = contexts;