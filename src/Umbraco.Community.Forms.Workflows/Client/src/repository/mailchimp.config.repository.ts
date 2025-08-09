import { UmbRepositoryBase } from "@umbraco-cms/backoffice/repository";
import type { UmbControllerHost } from '@umbraco-cms/backoffice/controller-api';
import { tryExecute } from "@umbraco-cms/backoffice/resources";
import { Config } from "../generated";

export class MailChimpConfigRepository extends UmbRepositoryBase {
  constructor(host: UmbControllerHost) {
    super(host);
  }

    async mailingLists(apiKey?: string) {

        const { data, error } = await tryExecute(
            this,
            Config.getMailChimpLists({
              query: {
                apiKey: apiKey
              } 
            })
        );


        if (data) {
        return { data };
        }

        return { error };
    }
}

export { MailChimpConfigRepository as api };