import {IBackendClient} from "@/spi/BackendClientSPI";
import {inject, injectable} from "inversify";
import Delivery, {DeliveryCrud} from "../models/Delivery";
import {createApiClient, schemas} from "@/infra/openAPI/client";
import axios from "axios";
import DeliveryMapper from "./DeliveryMapper";
import {hateoasRel, Link} from "@/models/HateoasLink";
import {ILogger} from "@/spi/LogsSPI";
import {ServicesIdentifiers} from "@/bootstrapper/constants/ServicesIdentifiers";
import {Step} from "@/steps/models/Step";
import JsonPatchDocument from "@/models/JsonPatchDocument";
import {IDeliveryCustomBackendClientFacade} from "@/deliveries/spi/IDeliveryCustomBackendClientFacade";

@injectable()
export default class DeliveryBackendClientFacade implements IBackendClient<Delivery>, IDeliveryCustomBackendClientFacade {
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
        const deliveryData = {
            ...item,
            contractDate: new Date(item.contractDate).toISOString(),
            startDate: new Date(item.startDate).toISOString(),
            steps: item.steps.map(step => ({
                data: {
                    ...step,
                    estimatedDeliveryDate: new Date(step.estimatedDeliveryDate).toISOString(),
                    realDeliveryDate: step.realDeliveryDate ? new Date(step.realDeliveryDate).toISOString() : null,
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

    public async PatchStepEndpoint(step: Step, json: JsonPatchDocument): Promise<void> {
        if (!step.links)
            throw new Error(`Step links empty`);

        const link = step.links.find(link => link.rel === hateoasRel.patch);
        if (!link)
            throw new Error(`Step link for '${hateoasRel.patch}' not found`);
        
        await axios.patch(
            link.href,
            json.getOperations(),
            {
                headers: {
                    "Content-Type": "application/json-patch+json"
                }
            }
        );
    }

    public async PostStepCourierEndpoint(step: Step): Promise<void> {
        if (!step.links)
            throw new Error(`Step links empty`);

        const link = step.links.find(link => link.rel === hateoasRel.stepCourier.post);
        if (!link)
            throw new Error(`Step link for '${hateoasRel.stepCourier.post}' not found`);
        
        if (!step.courierId)
            throw new Error(`Step courierId empty`);

        link.href = link.href.substring(0, link.href.lastIndexOf("/") + 1);
        link.href += step.courierId;
        
        await axios.post(
            link.href
        );
    }
    public async DeleteStepCourierEndpoint(step: Step): Promise<void> {
        if (!step.links)
            throw new Error(`Step links empty`);

        const link = step.links.find(link => link.rel === hateoasRel.stepCourier.delete);
        if (!link)
            throw new Error(`Step link for '${hateoasRel.stepCourier.delete}' not found`);

        await axios.delete(
            link.href
        );
    }
    
    public async PutStepOrderEndpoint(step: Step, increment: number): Promise<void> {
        if (!step.links)
            throw new Error(`Step links empty`);

        const link = step.links.find(link => link.rel === hateoasRel.stepOrder.put);
        if (!link)
            throw new Error(`Step link for '${hateoasRel.stepOrder.put}' not found`);

        await axios.put(
            link.href,
            {
                increment
            }
        );
    }

    public async PutStepTimeEndpoint(step: Step): Promise<void> {
        if (!step.links)
            throw new Error(`Step links empty`);

        const link = step.links.find(link => link.rel === hateoasRel.stepTime.put);
        if (!link)
            throw new Error(`Step link for '${hateoasRel.stepTime.put}' not found`);

        await axios.put(
            link.href,
            {
                date: step.estimatedDeliveryDate
            }
        );
    }
}