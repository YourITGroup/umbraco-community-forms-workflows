export const manifests: Array<UmbExtensionManifest> = [
  {
    name: "Umbraco Forms Community Workflows Entrypoint",
    alias: "Umbraco.Forms.Community.Workflows.Entrypoint",
    type: "backofficeEntryPoint",
    js: () => import("./entrypoint"),
  }
];
