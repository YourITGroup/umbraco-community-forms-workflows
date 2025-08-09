import { UmbRepositoryBase } from "@umbraco-cms/backoffice/repository";
import type { UmbControllerHost } from '@umbraco-cms/backoffice/controller-api';
import { tryExecute } from "@umbraco-cms/backoffice/resources";
import { Config } from "../generated";

export class MailcoachConfigRepository extends UmbRepositoryBase {
  constructor(host: UmbControllerHost) {
    super(host);
  }

    async mailingLists(domain?: string, token?: string) {

        const { data, error } = await tryExecute(
            this,
            Config.getMailCoachLists({
              query: {
                domain: domain,
                token: token
              }
            })
        );


        if (data) {
        return { data };
        }

        return { error };
    }
}

export { MailcoachConfigRepository as api };