export const manifests: Array<UmbExtensionManifest> = [
  {
    name: "Umbraco Forms Community Mailcoach Entrypoint",
    alias: "Umbraco.Forms.Community.Mailcoach.Workflow.Entrypoint",
    type: "backofficeEntryPoint",
    js: () => import("./entrypoint"),
  }
];
