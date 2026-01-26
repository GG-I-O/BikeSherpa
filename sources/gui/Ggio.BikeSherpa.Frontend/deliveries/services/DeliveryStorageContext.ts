import { Delivery, DeliveryDto } from "../models/Delivery";
import { inject, injectable } from "inversify";
import { ServicesIdentifiers } from "@/bootstrapper/constants/ServicesIdentifiers";
import { ILogger } from "@/spi/LogsSPI";
import AbstractStorageContext from "@/infra/storage/AbstractStorageContext";
import { INotificationService } from "@/spi/StorageSPI";
import { getTagByAlias } from "@/infra/openAPI/client";
import axios from "axios";
import { IBackendClient } from "@/spi/BackendClientSPI";
import DeliveryMapper from "./DeliveryMapper";

@injectable()
export default class DeliveryStorageContext extends AbstractStorageContext<Delivery> {
    private backendClient;

    public constructor(
        @inject(ServicesIdentifiers.Logger) logger: ILogger,
        @inject(ServicesIdentifiers.NotificationService) notificationService: INotificationService,
        @inject(ServicesIdentifiers.DeliveryBackendClientFacade) deliveryBackendClientFacade: IBackendClient<Delivery>
    ) {
        const tag = getTagByAlias("CreateDelivery") || "Delivery";
        super(tag, logger, notificationService);
        this.backendClient = deliveryBackendClientFacade;
    }

    protected async getList(lastSync?: string): Promise<Delivery[]> {
        return await this.backendClient.GetAllEndpoint(lastSync);
    }

    protected async getItem(id: string): Promise<Delivery | null> {
        let customer: Delivery | null;
        const link = this.getLinkHref(id, "self");
        if (link) {
            const response = await axios.get(link);
            const data = await response.data as DeliveryDto;
            customer = DeliveryMapper.DeliveryrDtoToDelivery(data);
            return customer;
        }
        else {
            return await this.backendClient.GetEndpoint(id);
        }
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