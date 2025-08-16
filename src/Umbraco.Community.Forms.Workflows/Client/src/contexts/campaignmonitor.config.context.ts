import type { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";
import { UmbContextBase } from "@umbraco-cms/backoffice/class-api";
import { CampaignMonitorConfigRepository } from "../repository/campaignmonitor.config.repository.js";
import type { BasicClient, BasicList } from "../generated/types.gen.js";
import { CAMPAIGNMONITOR_CONFIG_CONTEXT } from "./campaignmonitor.config.context.token.js";

export class CampaignMonitorConfigContext extends UmbContextBase {
  
  #repository: CampaignMonitorConfigRepository;
  #clients?: BasicClient[];
  #mailingLists?: BasicList[];
  #apiKey?: string = undefined
  #clientId?: string = undefined

  constructor(host: UmbControllerHost) {
    super(host, CAMPAIGNMONITOR_CONFIG_CONTEXT);
    this.#repository = new CampaignMonitorConfigRepository(host);
  }

  async getClients(apiKey?: string): Promise<BasicClient[] | undefined> {
    if (!this.#clients || this.#apiKey !== apiKey) {
      this.#apiKey = apiKey
      const result = await this.#repository.clients(this.#apiKey);
      if (result.data) {
        this.#clients = result.data;
        if (this.#clients?.length === 1) {
          await this.getMailingLists(this.#clients[0].clientID ?? undefined)
        }
        else {
          this.resetMailingLists()
        }
      }
    }
    return this.#clients;
  }

  async getMailingLists(clientId?: string): Promise<BasicList[] | undefined> {
    if (!this.#mailingLists || this.#clientId !== clientId) {
      this.#clientId = clientId
      const result = await this.#repository.mailingLists(this.#apiKey, this.#clientId);
      if (result.data) {
        this.#mailingLists = result.data;
      }
    }
    return this.#mailingLists;
  }

  resetMailingLists() {
    this.#mailingLists = undefined;
    this.#clientId = undefined
  }
}

export default CampaignMonitorConfigContext;