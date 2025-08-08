import { UmbRepositoryBase } from "@umbraco-cms/backoffice/repository";
import type { UmbControllerHost } from '@umbraco-cms/backoffice/controller-api';
import { tryExecute } from "@umbraco-cms/backoffice/resources";
import { Config } from "../api";

export class MailcoachConfigRepository extends UmbRepositoryBase {
  constructor(host: UmbControllerHost) {
    super(host);
  }

    async mailingLists() {

        const { data, error } = await tryExecute(
            this,
            Config.getMailingLists({})
        );


        if (data) {
        return { data };
        }

        return { error };
    }
}

export { MailcoachConfigRepository as api };