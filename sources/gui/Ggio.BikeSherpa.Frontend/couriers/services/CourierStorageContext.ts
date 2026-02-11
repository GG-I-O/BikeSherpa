import Courier, { CourierDto } from "../models/Courier";
import { inject, injectable } from "inversify";
import { ServicesIdentifiers } from "@/bootstrapper/constants/ServicesIdentifiers";
import { ILogger } from "@/spi/LogsSPI";
import AbstractStorageContext from "@/infra/storage/AbstractStorageContext";
import { INotificationService } from "@/spi/StorageSPI";
import { getTagByAlias } from "@/infra/openAPI/client";
import axios from "axios";
import CourierMapper from "./CourierMapper";
import { IBackendClient } from "@/spi/BackendClientSPI";

@injectable()
export default class CourierStorageContext extends AbstractStorageContext<Courier> {
    private backendClient;

    public constructor(
        @inject(ServicesIdentifiers.Logger) logger: ILogger,
        @inject(ServicesIdentifiers.NotificationService) notificationService: INotificationService,
        @inject(ServicesIdentifiers.CourierBackendClientFacade) courierBackendClientFacade: IBackendClient<Courier>
    ) {
        const tag = getTagByAlias("CreateCourier") || "Courier";
        super(tag, logger, notificationService);
        this.backendClient = courierBackendClientFacade;
    }

    protected async getList(lastSync?: string): Promise<Courier[]> {
        return await this.backendClient.GetAllEndpoint(lastSync);
    }

    protected async getItem(id: string): Promise<Courier | null> {
        let courier: Courier | null;
        const link = this.getLinkHref(id, "self");
        if (link) {
            const response = await axios.get(link);
            const data = await response.data as CourierDto;
            courier = CourierMapper.CourierDtoToCourier(data);
            return courier;
        }
        else {
            return await this.backendClient.GetEndpoint(id);
        }
    }

    protected async create(item: Courier): Promise<string> {
        return await this.backendClient.AddEndpoint(item);
    }

    protected async update(item: Courier): Promise<void> {
        return await this.backendClient.UpdateEndpoint(item);
    }

    protected async delete(item: Courier): Promise<void> {
        return await this.backendClient.DeleteEndpoint(item);
    }
}