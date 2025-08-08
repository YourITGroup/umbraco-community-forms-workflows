export const manifests: Array<UmbExtensionManifest> = [
  {
    type: 'repository',
    alias: 'Umbraco.Forms.Community.Mailcoach.Repository.Config',
    name: 'Umbraco Forms Community Mailcoach Config Repository',
    api: () => import('./config.repository.js'),
  }
];