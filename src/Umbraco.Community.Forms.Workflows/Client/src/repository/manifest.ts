export const manifests: Array<UmbExtensionManifest> = [
  {
    type: 'repository',
    alias: 'Umbraco.Forms.Community.Mailcoach.Repository.Config',
    name: 'Umbraco Forms Community Mailcoach Config Repository',
    api: () => import('./mailcoach.config.repository.js'),
  },
  {
    type: 'repository',
    alias: 'Umbraco.Forms.Community.MailChimp.Repository.Config',
    name: 'Umbraco Forms Community MailChimp Config Repository',
    api: () => import('./mailchimp.config.repository.js'),
  },
  {
    type: 'repository',
    alias: 'Umbraco.Forms.Community.CampaignMonitor.Repository.Config',
    name: 'Umbraco Forms Community CampaignMonitor Config Repository',
    api: () => import('./campaignmonitor.config.repository.js'),
  },
];