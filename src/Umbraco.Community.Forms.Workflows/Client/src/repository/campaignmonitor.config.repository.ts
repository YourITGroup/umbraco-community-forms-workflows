import { UmbRepositoryBase } from "@umbraco-cms/backoffice/repository";
import type { UmbControllerHost } from '@umbraco-cms/backoffice/controller-api';
import { tryExecute } from "@umbraco-cms/backoffice/resources";
import { Config } from "../generated";

export class CampaignMonitorConfigRepository extends UmbRepositoryBase {
  constructor(host: UmbControllerHost) {
    super(host);
  }

    async clients(apiKey?: string) {

        const { data, error } = await tryExecute(
            this,
            Config.getCampaignMonitorClients({
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

    async mailingLists(apiKey?: string, clientId?: string) {

        const { data, error } = await tryExecute(
            this,
            Config.getCampaignMonitorLists({
              query: {
                apiKey: apiKey,
                clientId: clientId
              }
            })
        );


        if (data) {
        return { data };
        }

        return { error };
    }
}

export { CampaignMonitorConfigRepository as api };