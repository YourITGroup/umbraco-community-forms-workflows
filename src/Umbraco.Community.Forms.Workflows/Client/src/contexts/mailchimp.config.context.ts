import type { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";
import { UmbContextBase } from "@umbraco-cms/backoffice/class-api";
import { MailChimpConfigRepository } from "../repository/mailchimp.config.repository.js";
import type { List } from "../generated/types.gen.js";
import { MAILCHIMP_CONFIG_CONTEXT } from "./mailchimp.config.context.token.js";

export class MailChimpConfigContext extends UmbContextBase {
  
  #repository: MailChimpConfigRepository;
  #mailingLists?: List[]
  #apiKey?: string = undefined

  constructor(host: UmbControllerHost) {
    super(host, MAILCHIMP_CONFIG_CONTEXT);
    this.#repository = new MailChimpConfigRepository(host);
  }

  async getMailingLists(apiKey?: string): Promise<List[] | undefined> {
    if (!this.#mailingLists || this.#apiKey !== apiKey) {
      this.#apiKey = apiKey
      const result = await this.#repository.mailingLists(apiKey);
      if (result.data) {
        this.#mailingLists = result.data;
      }
    }
    return this.#mailingLists;
  }

  // async refreshSettings(): Promise<List[] | undefined> {
  //   this.#mailingLists = undefined;
  //   return this.getMailingLists();
  // }
}

export default MailChimpConfigContext;