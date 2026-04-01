import { IStorageContext } from "@/spi/StorageSPI";
import { Container } from "inversify";
import Delivery from "../models/Delivery";
import { DeliveryServiceIdentifier } from "./DeliveryServiceIdentifier";
import InputDelivery from "../models/InputDelivery";
import { IBackendClient } from "@/spi/BackendClientSPI";
import DeliveryStorageContext from "../services/DeliveryStorageContext";
import DeliveryBackendClientFacade from "../services/DeliveryBackendClientfacade";
import DeliveryServices from "../services/DeliveryServices";
import { IDeliveryServices } from "../spi/IDeliveryServices";

export default class DeliveryBootstrapper {
    public static init(IOCContainer: Container) {
        // Storage
        IOCContainer.bind<IStorageContext<Delivery>>(DeliveryServiceIdentifier.Storage).to(DeliveryStorageContext).inSingletonScope();

        // Service
        IOCContainer.bind<IDeliveryServices>(DeliveryServiceIdentifier.Services).to(DeliveryServices).inSingletonScope();

        // BackendClientFacade
        IOCContainer.bind<IBackendClient<Delivery>>(DeliveryServiceIdentifier.BackendClientFacade).to(DeliveryBackendClientFacade).inSingletonScope();
    }
}