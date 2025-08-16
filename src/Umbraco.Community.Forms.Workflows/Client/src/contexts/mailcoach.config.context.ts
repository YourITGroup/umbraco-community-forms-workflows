import type { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";
import { UmbContextBase } from "@umbraco-cms/backoffice/class-api";
import { MailcoachConfigRepository } from "../repository/mailcoach.config.repository.js";
import type { EmailList } from "../generated/types.gen.js";
import { MAILCOACH_CONFIG_CONTEXT } from "./mailcoach.config.context.token.js";

export class MailcoachConfigContext extends UmbContextBase {
  
  #repository: MailcoachConfigRepository;
  #mailingLists?: EmailList[];
  #domain?: string = undefined
  #token?: string = undefined

  constructor(host: UmbControllerHost) {
    super(host, MAILCOACH_CONFIG_CONTEXT);
    this.#repository = new MailcoachConfigRepository(host);
  }

  async getMailingLists(domain?: string, token?: string): Promise<EmailList[] | undefined> {
    if (!this.#mailingLists || this.#domain !== domain || this.#token !== token) {
      this.#domain = domain
      this.#token = token
      const result = await this.#repository.mailingLists(domain, token);
      if (result.data) {
        this.#mailingLists = result.data;
      }
    }
    return this.#mailingLists;
  }

  // async refreshSettings(): Promise<MailcoachEmailList[] | undefined> {
  //   this.#mailingLists = undefined;
  //   return this.getMailingLists();
  // }
}

export default MailcoachConfigContext;