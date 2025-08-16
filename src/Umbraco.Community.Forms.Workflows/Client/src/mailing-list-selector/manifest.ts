export const manifests: Array<UmbExtensionManifest> = [
  {
    type: "propertyEditorUi",
    alias:
      "Umbraco.Forms.Community.Workflows.PropertyEditorUi.MailcoachMailingList",
    name: "Umbraco Forms Community Workflows Mailcoach Mailing List Property Editor UI",
    element: () =>
      import("./mailcoach-mailing-list-property-editor.element.js"),
    meta: {
      label: "Mailing List",
      icon: "icon-select",
      group: "common",
    },
  },
  {
    type: "propertyEditorUi",
    alias:
      "Umbraco.Forms.Community.Workflows.PropertyEditorUi.MailChimpMailingList",
    name: "Umbraco Forms Community Workflows MailChimp Mailing List Property Editor UI",
    element: () =>
      import("./mailchimp-mailing-list-property-editor.element.js"),
    meta: {
      label: "Mailing List",
      icon: "icon-select",
      group: "common",
    },
  },
  {
    type: "propertyEditorUi",
    alias:
      "Umbraco.Forms.Community.Workflows.PropertyEditorUi.CampaignMonitorMailingList",
    name: "Umbraco Forms Community Workflows CampaignMonitor Mailing List Property Editor UI",
    element: () =>
      import("./campaignmonitor-mailing-list-property-editor.element.js"),
    meta: {
      label: "Mailing List",
      icon: "icon-select",
      group: "common",
    },
  },
]
