import {
  html,
  css,
  customElement,
  property,
  when,
  ifDefined,
} from "@umbraco-cms/backoffice/external/lit"
import {choose} from 'lit/directives/choose.js';
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api"
import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element"
import type { UmbPropertyEditorUiElement } from "@umbraco-cms/backoffice/property-editor"
import type {
  UUIInputEvent,
  UUISelectEvent,
} from "@umbraco-cms/backoffice/external/uui"
import { UmbChangeEvent } from "@umbraco-cms/backoffice/event"
import { CAMPAIGNMONITOR_CONFIG_CONTEXT } from "../contexts/campaignmonitor.config.context.token"
import type { BasicList, BasicClient } from "../generated/types.gen"

export type CampaignMonitorMailingList = {
  apiKey?: string
  clientId?: string
  listId?: string
}

@customElement("campaignmonitor-mailing-list-property-editor")
export class CampaignMonitorMailingListPropertyEditorElement
  extends UmbElementMixin(UmbLitElement)
  implements UmbPropertyEditorUiElement
{
  #configContext?: typeof CAMPAIGNMONITOR_CONFIG_CONTEXT.TYPE

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

  #mailingListConfig: CampaignMonitorMailingList = {}

  #clients: Array<BasicClient> = []
  #mailingLists: Array<BasicList> = []

  constructor() {
    super()
    this.consumeContext(CAMPAIGNMONITOR_CONFIG_CONTEXT, (instance) => {
      this.#configContext = instance
      this.#fetchClients()
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

  async #fetchClients() {
    if (this.#configContext) {
      this.#clients =
        (await this.#configContext.getClients(this.#mailingListConfig.apiKey)) ?? []
      if (this.#clients.length === 1) {
        this.#mailingListConfig.clientId = this.#clients[0].clientID ?? undefined
        this.#fetchLists()
      }
      this.requestUpdate()
    }
  }

  async #fetchLists() {
    if (this.#configContext) {
      this.#mailingLists =
        (await this.#configContext.getMailingLists(this.#mailingListConfig.clientId)) ?? []
      this.requestUpdate()
    }
  }

  #onApiKeyChange(e: UUIInputEvent) {
    this.#mailingListConfig.apiKey = e.target.value.toString()
    // We need to re-query the clients and mailing lists.
    this.#fetchClients()
  }

  #onClientChange(e: UUISelectEvent) {
    this.#mailingListConfig.clientId = e.target.value.toString()
    // We need to re-query the mailing lists.
    this.#fetchLists()
  }

  #onListChange(e: UUISelectEvent) {
    this.#mailingListConfig.listId = e.target.value.toString()
    this.#refreshValue()
  }

  #refreshValue() {
    this.value = JSON.stringify(this.#mailingListConfig)
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
        <uui-label for="apiKey">API Key</uui-label>
        <uui-textarea
          id="apiKey"
          name="apiKey"
          label="API Key"
          auto-height=""
          placeholder="CampaignMonitor API Key"
          .value=${ifDefined(this.#mailingListConfig.apiKey)}
          @change=${(e: UUIInputEvent) => this.#onApiKeyChange(e)}>
        </uui-textarea>
      </div>
      ${choose(
        this.#clients.length,
        [
          [0, () => html`<p>No client found for API Key</p>`],
          [
            1,
            () =>
              html`
                <div class="umb-forms-mailing-list-field">
                  <uui-label for="client">Client</uui-label>
                  ${this.#clients[0].clientID}<br />${this.#clients[0].name}
                </div>
              `,
          ],
        ],
        () =>
          html`
            <div class="umb-forms-mailing-list-field">
              <uui-label for="client">Client</uui-label>
              <uui-select
                id="client"
                name="client"
                label="Client"
                placeholder="Select a Client"
                @change=${(e: UUISelectEvent) => this.#onClientChange(e)}
                .options=${this.#clients.map((l) => {
                  return {
                    name: l.name,
                    value: l.clientID,
                    selected:
                      l.clientID === this.#mailingListConfig.clientId
                        ? true
                        : undefined,
                  }
                })}>
              </uui-select>
            </div>
          `
      )}
      <div class="umb-forms-mailing-list-field">
        ${when(
          this.#mailingLists.length > 0,
          () =>
            html`
              <uui-label for="mailingList">Mailing List</uui-label>
              <uui-select
                id="mailingList"
                name="mailingList"
                label="Mailing List"
                placeholder="Select a mailing list"
                @change=${(e: UUISelectEvent) => this.#onListChange(e)}
                .options=${this.#mailingLists.map((l) => {
                  return {
                    name: l.name,
                    value: l.listID,
                    selected:
                      l.listID === this.#mailingListConfig.listId
                        ? true
                        : undefined,
                  }
                })}>
              </uui-select>
            `,
          () =>
            html`<p>
              A valid API Key and Client must be configured to display available
              Mailing Lists
            </p>`
        )}
      </div>
    </div>`
  }
}

export default CampaignMonitorMailingListPropertyEditorElement

declare global {
  interface HTMLElementTagNameMap {
    "campaignmonitor-mailing-list-property-editor": CampaignMonitorMailingListPropertyEditorElement
  }
}
