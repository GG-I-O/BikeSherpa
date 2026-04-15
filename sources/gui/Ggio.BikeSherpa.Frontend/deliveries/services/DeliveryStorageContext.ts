import AbstractStorageContext from "@/infra/storage/AbstractStorageContext";
import { inject, injectable } from "inversify";
import Delivery from "../models/Delivery";
import { ServicesIdentifiers } from "@/bootstrapper/constants/ServicesIdentifiers";
import { INotificationService } from "@/spi/StorageSPI";
import { DeliveryServiceIdentifier } from "../bootstrapper/DeliveryServiceIdentifier";
import { IBackendClient } from "@/spi/BackendClientSPI";
import { getTagByAlias } from "@/infra/openAPI/client";
import { ILogger } from "@/spi/LogsSPI";

@injectable()
export default class DeliveryStorageContext extends AbstractStorageContext<Delivery> {
    private backendClient;

    public constructor(
        @inject(ServicesIdentifiers.Logger) logger: ILogger,
        @inject(ServicesIdentifiers.NotificationService) notificationService: INotificationService,
        @inject(DeliveryServiceIdentifier.BackendClientFacade) deliveryBackendClientFacade: IBackendClient<Delivery>
    ) {
        const tag = getTagByAlias("CreateDelivery") || "Delivery";
        super(tag, logger, notificationService);
        this.backendClient = deliveryBackendClientFacade;
    }

    protected async getList(lastSync?: string): Promise<Delivery[]> {
        return await this.backendClient.GetAllEndpoint(lastSync);
    }
    protected async getItem(id: string): Promise<Delivery | null> {
        return await this.backendClient.GetEndpoint(id);
    }
    protected async create(item: Delivery): Promise<string> {
        return await this.backendClient.AddEndpoint(item);
    }
    protected async update(item: Delivery): Promise<void> {
        return await this.backendClient.UpdateEndpoint(item);
    }
    protected async delete(item: Delivery): Promise<void> {
        return await this.backendClient.DeleteEndpoint(item);
    }

}