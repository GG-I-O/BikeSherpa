import {IBackendClient} from "@/spi/BackendClientSPI";
import {inject, injectable} from "inversify";
import Delivery, {DeliveryCrud} from "../models/Delivery";
import {createApiClient, schemas} from "@/infra/openAPI/client";
import axios from "axios";
import DeliveryMapper from "./DeliveryMapper";
import {Link} from "@/models/HateoasLink";
import deliveryOperationAction from "@/steps/constants/deliveryOperationAction";
import {ILogger} from "@/spi/LogsSPI";
import {ServicesIdentifiers} from "@/bootstrapper/constants/ServicesIdentifiers";
import {Step} from "@/steps/models/Step";
import JsonPatchDocument from "@/models/JsonPatchDocument";

@injectable()
export default class DeliveryBackendClientFacade implements IBackendClient<Delivery> {
    private apiClient;
    private logger: ILogger;

    public constructor(
        @inject(ServicesIdentifiers.Logger) logger: ILogger
    ) {
        this.apiClient = createApiClient(axios.defaults.baseURL || '', {
            axiosInstance: axios
        });
        this.logger = logger;
        this.logger = this.logger.extend("DeliveryBackendClientFacade");
    }

    public async GetAllEndpoint(lastSync?: string): Promise<Delivery[]> {
        const data = await this.apiClient.GetAllDeliveriesEndpoint({
            params: {lastSync: lastSync ?? ''}
        });

        const deliveries = data.map((deliveryDto: { data: DeliveryCrud, links: Link[] | null }) => {
            return DeliveryMapper.DeliveryDtoToDelivery(deliveryDto);
        });

        return deliveries || [];
    }

    public async GetEndpoint(id: string): Promise<Delivery | null> {
        const response = await this.apiClient.GetDeliveryEndpoint({
            params: {deliveryId: id}
        });
        return DeliveryMapper.DeliveryDtoToDelivery(response);
    }

    public async AddEndpoint(item: Delivery): Promise<string> {
        // Creating Delivery

        // Fill fields for Zod
        const deliveryData = {
            ...item,
            contractDate: new Date(item.contractDate).toISOString(),
            startDate: new Date(item.startDate).toISOString(),
            steps: []
        };
        const parsedDelivery = schemas.DeliveryCrud.safeParse(deliveryData);
        if (!parsedDelivery.success) {
            this.logger.error("Parsing Create Delivery error:");
            this.logger.error(parsedDelivery.error.format());
            throw parsedDelivery.error;
        }

        const deliveryResponse = await this.apiClient.AddDeliveryEndpoint(
            parsedDelivery.data,
            {
                headers: {operationId: item.operationId}
            }
        );

        // Creating steps associated
        for (let i = 0; i < item.steps.length; i++) {
            const step = {
                ...item.steps[i],
                estimatedDeliveryDate: new Date().toISOString(),
                createdAt: new Date().toISOString(),
                updatedAt: new Date().toISOString()
            };
            const parsedStep = schemas.DeliveryStep.safeParse(step);
            if (!parsedStep.success) {
                this.logger.error("Parsing Create Step error:");
                this.logger.error(parsedStep.error.format());
                throw parsedStep.error;
            }

            await this.apiClient.AddDeliveryStepEndpoint(
                parsedStep.data,
                {
                    params: {deliveryId: deliveryResponse.id},
                    headers: {operationId: item.operationId}
                }
            );
        }

        return deliveryResponse.id;
    }

    public async DeleteEndpoint(item: Delivery): Promise<void> {
        await this.apiClient.DeleteDeliveryEndpoint(
            undefined,
            {
                params: {deliveryId: item.id},
                headers: {operationId: item.operationId}
            }
        );
    }

    public async UpdateEndpoint(item: Delivery): Promise<void> {
        const step = item.steps.find(step => step.operationAction !== undefined);

        if (!step)
            await this.UpdateDelivery(item);
        else {
            switch (step.operationAction) {
                case deliveryOperationAction.patchTime:
                    await this.PatchStepTime(step);
                    break;
                default:
                    throw new Error(`Unsupported operation action: ${step.operationAction}`);
            }
            step.operationAction = undefined;
        }
    }

    private async UpdateDelivery(item: Delivery): Promise<void> {
        const deliveryData = {
            ...item,
            contractDate: new Date(item.contractDate).toISOString(),
            startDate: new Date(item.startDate).toISOString(),
            steps: item.steps.map(step => ({
                data: {
                    ...step,
                    createdAt: step.createdAt ? step.createdAt : new Date().toISOString(),
                    updatedAt: step.updatedAt ? step.updatedAt : new Date().toISOString()
                },
                links: []
            }))
        };
        const parsedDelivery = schemas.DeliveryCrud.safeParse(deliveryData);
        if (!parsedDelivery.success) {
            this.logger.error("Update Debug (DeliveryCrud validation failed):");
            this.logger.error(parsedDelivery.error.format());
            throw parsedDelivery.error;
        }

        const delivery = parsedDelivery.data;
        await this.apiClient.UpdateDeliveryEndpoint(
            delivery,
            {
                params: {deliveryId: delivery.id},
                headers: {operationId: item.operationId}
            }
        );
    }

    private async PatchStepTime(step: Step): Promise<void> {
        if (!step.links)
            throw new Error(`Step links empty`);

        const url = step.links.find(link => link.rel === step.operationAction);
        if (!url)
            throw new Error(`Step operationAction '${step.operationAction}' not found in links`);

        let jsonPatchDocument = new JsonPatchDocument();
        jsonPatchDocument.addOperation(
            "/estimatedDeliveryDate",
            "replace",
            new Date(step.estimatedDeliveryDate).toISOString()
        );
        await axios.patch(
            url.href,
            jsonPatchDocument.getOperations(),
            {
                headers: {
                    "Content-Type": "application/json-patch+json"
                }
            }
        );
    }
}