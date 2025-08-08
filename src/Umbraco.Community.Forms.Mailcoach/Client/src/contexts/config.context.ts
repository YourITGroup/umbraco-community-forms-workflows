import type { UmbControllerHost } from "@umbraco-cms/backoffice/controller-api";
import { UmbContextBase } from "@umbraco-cms/backoffice/class-api";
import { MailcoachConfigRepository } from "../repository/config.repository.js";
import type { MailcoachEmailList } from "../api/types.gen.js";

export class MailcoachConfigContext extends UmbContextBase {
  
  #repository: MailcoachConfigRepository;
  #mailingLists?: MailcoachEmailList[];

  constructor(host: UmbControllerHost) {
    super(host, 'GMapsConfigContext');
    this.#repository = new MailcoachConfigRepository(host);
  }

  async getMailingLists(): Promise<MailcoachEmailList[] | undefined> {
    if (!this.#mailingLists) {
      const result = await this.#repository.mailingLists();
      if (result.data) {
        this.#mailingLists = result.data;
      }
    }
    return this.#mailingLists;
  }

  async refreshSettings(): Promise<MailcoachEmailList[] | undefined> {
    this.#mailingLists = undefined;
    return this.getMailingLists();
  }
}

export default MailcoachConfigContext;