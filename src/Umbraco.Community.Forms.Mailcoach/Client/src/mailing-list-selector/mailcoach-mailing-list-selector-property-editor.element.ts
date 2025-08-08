import {
  html,
  css,
  customElement,
  property,
  state,
} from "@umbraco-cms/backoffice/external/lit"
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api"
import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element"
import type { UmbPropertyEditorUiElement } from "@umbraco-cms/backoffice/property-editor"
import { MailcoachConfigContext } from "../contexts/config.context"
import type { UUISelectEvent } from "@umbraco-cms/backoffice/external/uui"
import { UmbChangeEvent } from "@umbraco-cms/backoffice/event"

@customElement("mailcoach-mailing-list-selector-property-editor")
export class MailcoachMailingListPropertyEditorElement
  extends UmbElementMixin(UmbLitElement)
  implements UmbPropertyEditorUiElement
{
  #configContext?: MailcoachConfigContext

  @property({ type: String })
  value = ""

  @state()
  private _mailingLists?: Array<Option>

  constructor() {
    super()

    this.#configContext = new MailcoachConfigContext(this)
  }

  async firstUpdated() {
    if (this.#configContext) {
      const mailingLists = await this.#configContext.getMailingLists()
      if (mailingLists) {
        this._mailingLists = mailingLists.map((l) => {
          return { name: l.name, value: l.uuid, selected: l.uuid === this.value ? true : undefined }
        })
      }
    }
  }

  #onSelectChange(e: UUISelectEvent) {
    this.value = e.target.value.toString()
    this.#refreshValue()
  }

  #refreshValue() {
    this.dispatchEvent(new UmbChangeEvent());
  }

  static styles = css`
    :host {
      display: block;
    }
  `
  
  render() {
    return html`
      <uui-select
        @change=${(e: UUISelectEvent) => this.#onSelectChange(e)}
        .options=${this._mailingLists}
        label="Mailing Lists"
        placeholder="Select a mailing list">
      </uui-select>
    `
  }
}

export default MailcoachMailingListPropertyEditorElement

declare global {
  interface HTMLElementTagNameMap {
    "mailcoach-mailing-list-selector-property-editor": MailcoachMailingListPropertyEditorElement
  }
}
