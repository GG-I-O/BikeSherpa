import { IStorageContext } from "@/spi/StorageSPI";
import { Container } from "inversify";
import Delivery from "../models/Delivery";
import { DeliveryServiceIdentifier } from "./DeliveryServiceIdentifier";
import { IBackendClient } from "@/spi/BackendClientSPI";
import DeliveryStorageContext from "../services/DeliveryStorageContext";
import DeliveryBackendClientFacade from "../services/DeliveryBackendClientfacade";
import DeliveryServices from "../services/DeliveryServices";
import { IDeliveryServices } from "../spi/IDeliveryServices";
import {IDropdownOptions} from "@/spi/IDropdownOptions";
import DeliveryDropdownOptionsService from "@/deliveries/services/DeliveryDropdownOptionsService";
import {IDeliveryStorageMiddleware} from "@/deliveries/spi/IDeliveryStorageMiddleware";
import DeliveryStorageMiddleware from "@/deliveries/services/DeliveryStorageMiddleware";
import {IDeliveryCustomBackendClientFacade} from "@/deliveries/spi/IDeliveryCustomBackendClientFacade";

export default class DeliveryBootstrapper {
    public static init(IOCContainer: Container) {
        // Storage
        IOCContainer.bind<IStorageContext<Delivery>>(DeliveryServiceIdentifier.Storage).to(DeliveryStorageContext).inSingletonScope();

        // Service
        IOCContainer.bind<IDeliveryServices>(DeliveryServiceIdentifier.Services).to(DeliveryServices).inSingletonScope();

        // BackendClientFacade
        IOCContainer.bind<IBackendClient<Delivery>>(DeliveryServiceIdentifier.BackendClientFacade).to(DeliveryBackendClientFacade).inSingletonScope();
        IOCContainer.bind<IDeliveryCustomBackendClientFacade>(DeliveryServiceIdentifier.CustomBackendClientFacade).to(DeliveryBackendClientFacade).inSingletonScope();

        // DropdownOptionsService
        IOCContainer.bind<IDropdownOptions<Delivery>>(DeliveryServiceIdentifier.DropdownOptionsService).to(DeliveryDropdownOptionsService).inSingletonScope();
        
        // StorageMiddleware
        IOCContainer.bind<IDeliveryStorageMiddleware>(DeliveryServiceIdentifier.StorageMiddleware).to(DeliveryStorageMiddleware).inSingletonScope();
    }
}