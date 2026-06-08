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
        const deliveryCrud = this.mapDeliveryToDeliveryCrud(delivery);
        return await this.apiClient.CalculateDeliveryPriceEndpoint(deliveryCrud);
    }

    public getVatRate = async (): Promise<number> => {
        return await this.apiClient.GetParameterVatRateEndpoint();
    }
    public getLastHourToOrder = async (): Promise<number> => {
        return await this.apiClient.GetParameterLastHourToOrderEndpoint();
    }
    public getUrgenciesLastHourToOrder = async (): Promise<{
        value: string,
        label: string,
        lastHourToOrder: number
    }[]> => {
        return await this.apiClient.GetAllUrgenciesEndpoint();
    }
    public getWorkHours = async (): Promise<{ startDate: string, endDate: string }> => {
        return await this.apiClient.GetParameterWorkHoursEndpoint();
    }

    public createDelivery = async (delivery: Delivery, customer: Customer): Promise<boolean> => {
        try {
            const deliveryCrud = this.mapDeliveryToDeliveryCrud(delivery);
            const customerCrud = this.mapCustomerToCustomerCrud(customer);
            await this.apiClient.AddDeliveryByCustomerEndpoint({delivery: deliveryCrud, customer: customerCrud});
            return true;
        } catch (error) {
            this.logger.error("Create delivery by customer error:", error);
            return false;
        }
    }

    private mapDeliveryToDeliveryCrud = (delivery: Delivery) => {
        let steps = [];
        for (let i = 0; i < delivery.steps.length; i++) {
            const step = {
                ...delivery.steps[i],
                order: i + 1,
                estimatedDeliveryDate: new Date().toISOString(),
                createdAt: new Date().toISOString(),
                updatedAt: new Date().toISOString()
            };
            const parsedStep = schemas.DeliveryStepCrud.safeParse(step);
            if (!parsedStep.success) {
                this.logger.error("Parsing Step error:");
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
            this.logger.error("Parsing Delivery error:");
            this.logger.error(parsedDelivery.error.format());
            throw parsedDelivery.error;
        }

        return parsedDelivery.data;
    }

    private mapCustomerToCustomerCrud = (customer: Customer) => {
        const customerData = {
            ...customer,
            siret: customer.siret ?? null,
            vatNumber: customer.vatNumber ?? null,
            address: {
                ...customer.address,
                complement: customer.address.complement ?? ""
            },
            createdAt: new Date().toISOString(),
            updatedAt: new Date().toISOString(),
            defaultDeliveryType: 0
        }
        const parsedCustomer = schemas.CustomerCrud.safeParse(customerData);
        if (!parsedCustomer.success) {
            this.logger.error("Parsing Customer error:");
            this.logger.error(parsedCustomer.error.format());
            throw parsedCustomer.error;
        }

        return parsedCustomer.data;
    }
}