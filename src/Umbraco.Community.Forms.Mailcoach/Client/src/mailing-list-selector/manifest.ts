export const manifests: Array<UmbExtensionManifest> = [{
    type: 'propertyEditorUi',
    alias: 'Umbraco.Forms.Community.Mailcoach.PropertyEditorUi.MailingListSelector',
    name: 'Umbraco Forms Community Mailcoach Mailing List Selector Property Editor UI',
    element: () => import('./mailcoach-mailing-list-selector-property-editor.element.js'),
    meta: {
        label: 'Mailing List',
        icon: 'icon-select',
        group: 'common',
    },
}];