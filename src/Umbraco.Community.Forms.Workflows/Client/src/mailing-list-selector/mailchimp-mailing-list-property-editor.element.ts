import {
  html,
  css,
  customElement,
  property,
  when,
  ifDefined,
} from "@umbraco-cms/backoffice/external/lit"
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api"
import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element"
import type { UmbPropertyEditorUiElement } from "@umbraco-cms/backoffice/property-editor"
import type {
  UUIInputEvent,
  UUISelectEvent,
} from "@umbraco-cms/backoffice/external/uui"
import { UmbChangeEvent } from "@umbraco-cms/backoffice/event"
import { MAILCHIMP_CONFIG_CONTEXT } from "../contexts/mailchimp.config.context.token"
import type { List } from "../generated/types.gen"

export type MailChimpMailingList = {
  apiKey?: string
  listId?: string
}

@customElement("mailchimp-mailing-list-property-editor")
export class MailChimpMailingListPropertyEditorElement
  extends UmbElementMixin(UmbLitElement)
  implements UmbPropertyEditorUiElement
{
  #configContext?: typeof MAILCHIMP_CONFIG_CONTEXT.TYPE

  #value: string = ""
  @property({ type: String })
  get value() {
    return this.#value
  }
  set value(value: string) {
    const oldVal = this.#value
    this.#value = value
    this.requestUpdate("value", oldVal)

    this.#initializeMailingListConfig(value)
  }

  #mailingListConfig: MailChimpMailingList = {}

  #mailingLists: Array<List> = []

  constructor() {
    super()
    this.consumeContext(MAILCHIMP_CONFIG_CONTEXT, (instance) => {
      this.#configContext = instance
      this.#fetchLists()
    })
  }

  #initializeMailingListConfig(value?: string) {
    if (value && value.length > 0) {
      try {
        this.#mailingListConfig = JSON.parse(value)
      } catch {}
    }
  }

  async #fetchLists() {
    if (this.#configContext) {
      this.#mailingLists =
        (await this.#configContext.getMailingLists(
          this.#mailingListConfig.apiKey
        )) ?? []
      this.requestUpdate()
    }
  }

  #onApiKeyChange(e: UUIInputEvent) {
    this.#mailingListConfig.apiKey = e.target.value.toString()
    // We need to re-query the mailing lists.
    this.#fetchLists()
  }

  #onSelectChange(e: UUISelectEvent) {
    this.#mailingListConfig.listId = e.target.value.toString()
    this.#refreshValue()
  }

  #refreshValue() {
    this.value = JSON.stringify(this.#mailingListConfig);
    this.dispatchEvent(new UmbChangeEvent())
  }

  static styles = css`
    :host {
      display: block;
    }
    .umb-forms-mailing-list {
      display: flex;
      flex-direction: column;
      gap: 0.5rem;
    }
    .umb-forms-mailing-list-field {
      display: flex;
      flex-direction: column;
      margin-bottom: 0.25rem;
      gap: 0.25rem;
    }
  `

  render() {
    return html`<div class="umb-forms-mailing-list">
      <div class="umb-forms-mailing-list-field">
        <uui-label for="apiKey">API Key</uui-label>
        <uui-input
          type="text"
          id="apiKey"
          label="API Key"
          auto-width=""
          placeholder="MailChimp API Key"
          .value=${ifDefined(this.#mailingListConfig.apiKey)}
          @change=${(e: UUIInputEvent) => this.#onApiKeyChange(e)}>
        </uui-input>
      </div>
      <div class="umb-forms-mailing-list-field">
        ${when(
          this.#mailingLists.length > 0,
          () =>
            html`<uui-label for="mailingList">Mailing List</uui-label>
              <uui-select
                id="mailingList"
                label="Mailing Lists"
                placeholder="Select a mailing list"
                @change=${(e: UUISelectEvent) => this.#onSelectChange(e)}
                .options=${this.#mailingLists.map((l) => ({
                  name: l.name!,
                  value: l.id!,
                  selected:
                    l.id === this.#mailingListConfig.listId ? true : undefined,
                }))}>
              </uui-select>`,
          () =>
            html`<p>
              A valid API key must be configured to display available Mailing Lists
            </p>`
        )}
      </div>
    </div>`
  }
}

export default MailChimpMailingListPropertyEditorElement

declare global {
  interface HTMLElementTagNameMap {
    "mailchimp-mailing-list-property-editor": MailChimpMailingListPropertyEditorElement
  }
}
