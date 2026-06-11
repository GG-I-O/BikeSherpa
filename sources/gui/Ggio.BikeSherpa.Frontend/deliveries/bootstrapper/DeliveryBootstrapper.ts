import { IStorageContext } from "@/spi/StorageSPI";
import { Container } from "inversify";
import Delivery from "../models/Delivery";
import { DeliveryServiceIdentifier } from "./DeliveryServiceIdentifier";
import { IBackendClient } from "@/spi/BackendClientSPI";
import DeliveryStorageContext from "../services/DeliveryStorageContext";
import DeliveryBackendClientFacade from "../services/DeliveryBackendClientfacade";
import DeliveryServices from "../services/DeliveryServices";
import { IDeliveryServices } from "../spi/IDeliveryServices";
import {IDeliveryStorageMiddleware} from "@/deliveries/spi/IDeliveryStorageMiddleware";
import DeliveryStorageMiddleware from "@/deliveries/services/DeliveryStorageMiddleware";
import {IDeliveryCustomBackendClientFacade} from "@/deliveries/spi/IDeliveryCustomBackendClientFacade";
import IDeliveryMapper from "@/deliveries/spi/IDeliveryMapper";
import DeliveryMapper from "@/deliveries/services/DeliveryMapper";
import IPublicDeliveryService from "@/deliveries/spi/IPublicDeliveryService";
import PublicDeliveryService from "@/deliveries/services/PublicDeliveryService";

export default class DeliveryBootstrapper {
    public static init(IOCContainer: Container) {
        // Storage
        IOCContainer.bind<IStorageContext<Delivery>>(DeliveryServiceIdentifier.Storage).to(DeliveryStorageContext).inSingletonScope();

        // Services
        IOCContainer.bind<IDeliveryServices>(DeliveryServiceIdentifier.Services).to(DeliveryServices).inSingletonScope();
        IOCContainer.bind<IPublicDeliveryService>(DeliveryServiceIdentifier.PublicServices).to(PublicDeliveryService).inSingletonScope();
        
        // Mapper
        IOCContainer.bind<IDeliveryMapper>(DeliveryServiceIdentifier.Mapper).to(DeliveryMapper).inSingletonScope();
        
        // BackendClientFacade
        IOCContainer.bind<IBackendClient<Delivery>>(DeliveryServiceIdentifier.BackendClientFacade).to(DeliveryBackendClientFacade).inSingletonScope();
        IOCContainer.bind<IDeliveryCustomBackendClientFacade>(DeliveryServiceIdentifier.CustomBackendClientFacade).to(DeliveryBackendClientFacade).inSingletonScope();
        
        // StorageMiddleware
        IOCContainer.bind<IDeliveryStorageMiddleware>(DeliveryServiceIdentifier.StorageMiddleware).to(DeliveryStorageMiddleware).inSingletonScope();
    }
}