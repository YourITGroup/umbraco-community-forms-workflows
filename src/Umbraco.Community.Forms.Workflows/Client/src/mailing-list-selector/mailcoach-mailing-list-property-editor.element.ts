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
import { MAILCOACH_CONFIG_CONTEXT } from "../contexts/mailcoach.config.context.token"
import type { EmailList } from "../generated/types.gen"

export type MailcoachMailingList = {
  domain?: string
  token?: string
  listId?: string
}

@customElement("mailcoach-mailing-list-property-editor")
export class MailcoachMailingListPropertyEditorElement
  extends UmbElementMixin(UmbLitElement)
  implements UmbPropertyEditorUiElement
{
  #configContext?: typeof MAILCOACH_CONFIG_CONTEXT.TYPE

  #value = ""
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

  #mailingListConfig: MailcoachMailingList = {}

  #mailingLists: Array<EmailList> = []

  constructor() {
    super()
    this.consumeContext(MAILCOACH_CONFIG_CONTEXT, (instance) => {
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
          this.#mailingListConfig.domain,
          this.#mailingListConfig.token
        )) ?? []
      this.requestUpdate()
    }
  }

  #onDomainChange(e: UUIInputEvent) {
    this.#mailingListConfig.domain = e.target.value.toString()
    // We need to re-query the mailing lists.
    this.#fetchLists()
  }

  #onTokenChange(e: UUIInputEvent) {
    this.#mailingListConfig.token = e.target.value.toString()
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
    return html` <div class="umb-forms-mailing-list">
      <div class="umb-forms-mailing-list-field">
        <uui-label for="domain">Domain</uui-label>
        <uui-input
          type="text"
          id="domain"
          label="Domain"
          auto-width=""
          placeholder="Mailcoach server domain name"
          .value=${ifDefined(this.#mailingListConfig.domain)}
          @change=${(e: UUIInputEvent) => this.#onDomainChange(e)}>
        </uui-input>
      </div>
      <div class="umb-forms-mailing-list-field">
        <uui-label for="token">API Token</uui-label>
        <uui-input
          type="text"
          id="token"
          label="Token"
          auto-width=""
          placeholder="Mailcoach API Token"
          .value=${ifDefined(this.#mailingListConfig.token)}
          @change=${(e: UUIInputEvent) => this.#onTokenChange(e)}>
        </uui-input>
      </div>
      <div class="umb-forms-mailing-list-field">
        ${when(
          this.#mailingLists.length > 0,
          () =>
            html`
              <uui-label for="mailingList">Mailing List</uui-label>
              <uui-select
                id="mailingList"
                label="Mailing Lists"
                placeholder="Select a mailing list"
                @change=${(e: UUISelectEvent) => this.#onSelectChange(e)}
                .options=${this.#mailingLists.map((l) => {
                  return {
                    name: l.name,
                    value: l.uuid,
                    selected:
                      l.uuid === this.#mailingListConfig.listId
                        ? true
                        : undefined,
                  }
                })}>
              </uui-select>
            `,
          () =>
            html`<p>
              A valid Token and Domain must be configured to display available Mailing
              Lists
            </p>`
        )}
      </div>
    </div>`
  }
}

export default MailcoachMailingListPropertyEditorElement

declare global {
  interface HTMLElementTagNameMap {
    "mailcoach-mailing-list-property-editor": MailcoachMailingListPropertyEditorElement
  }
}
