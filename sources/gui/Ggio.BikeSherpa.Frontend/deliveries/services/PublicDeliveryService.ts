import IPublicDeliveryService from "@/deliveries/spi/IPublicDeliveryService";
import {inject, injectable} from "inversify";
import {createApiClient, schemas} from "@/infra/openAPI/client";
import axios from "axios";
import Delivery from "@/deliveries/models/Delivery";
import Customer from "@/customers/models/Customer";
import {ILogger} from "@/spi/LogsSPI";
import {ServicesIdentifiers} from "@/bootstrapper/constants/ServicesIdentifiers";

@injectable()
export default class PublicDeliveryService implements IPublicDeliveryService {
    private apiClient;
    private logger: ILogger;

    public constructor(
        @inject(ServicesIdentifiers.Logger) logger: ILogger
    ) {
        this.apiClient = createApiClient(axios.defaults.baseURL || '', {
            axiosInstance: axios
        });
        this.logger = logger;
        this.logger = this.logger.extend("PublicDeliveryService");
    }

    public loginPublicDeliveryCustomer = async (email: string, code: string) => {
        try {
            const result = await this.apiClient.CheckCustomerEndpoint(undefined, {
                queries: {
                    email,
                    code
                }
            });

            return {
                name: result.customerName,
                deliveryType: result.defaultDeliveryType ?? 0
            }
        } catch (error) {
            this.logger.error("Login public delivery customer error:", error);
            return null;
        }
    }

    public getEstimatedValue = async (delivery: Delivery) => {
        // Creating steps associated
        let steps = [];
        for (let i = 0; i < delivery.steps.length; i++) {
            const step = {
                ...delivery.steps[i],
                estimatedDeliveryDate: new Date().toISOString(),
                createdAt: new Date().toISOString(),
                updatedAt: new Date().toISOString()
            };
            const parsedStep = schemas.DeliveryStepCrud.safeParse(step);
            if (!parsedStep.success) {
                this.logger.error("Parsing GetEstimatedValue Step error:");
                this.logger.error(parsedStep.error.format());
                throw parsedStep.error;
            }
            steps.push({...parsedStep, links: []});
        }
        
        const deliveryData = {
            ...delivery,
            contractDate: new Date(delivery.contractDate).toISOString(),
            startDate: new Date(delivery.startDate).toISOString(),
            limitDate: null,
            steps: steps,
            createdAt: new Date().toISOString(),
            updatedAt: new Date().toISOString()
        };
        const parsedDelivery = schemas.DeliveryCrud.safeParse(deliveryData);
        if (!parsedDelivery.success) {
            this.logger.error("Parsing GetEstimatedValue Delivery error:");
            this.logger.error(parsedDelivery.error.format());
            throw parsedDelivery.error;
        }
        
        return await this.apiClient.CalculateDeliveryPriceEndpoint(parsedDelivery.data);
    }

    public getVatRate = async (): Promise<number> => {
        return await this.apiClient.GetParameterVatRateEndpoint();
    }
    public getLastHourToOrder = async (): Promise<number> => {
        return await this.apiClient.GetParameterLastHourToOrderEndpoint();
    }
    public getUrgenciesLastHourToOrder = async (): Promise<{ value: string, label: string, lastHourToOrder: number }[]> => {
        return await this.apiClient.GetAllUrgenciesEndpoint();
    }
    public getWorkHours = async (): Promise<{startDate: string, endDate: string}> => {
        return await this.apiClient.GetParameterWorkHoursEndpoint();
    }

    public createDelivery = async (delivery: Delivery, customer: Customer): Promise<boolean> => {
        console.debug("createDelivery")
        console.debug(delivery)
        console.debug(customer)
        // const result = await this.apiClient.createPublicDelivery({params: {delivery: delivery, customer: customer} });
        return true;
    }
}