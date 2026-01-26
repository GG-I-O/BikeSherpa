import Customer from "@/customers/models/Customer";
import CustomerStorageContext from "@/customers/services/CustomerStorageContext";
import { ILogger, ILoggerConfig } from "@/spi/LogsSPI";
import { INotificationService, IStorageContext } from "@/spi/StorageSPI";
import { Platform } from "react-native";
import { IOCContainer } from "./constants/IOCContainer";
import { ServicesIdentifiers } from "./constants/ServicesIdentifiers";
import { IAuthService, IUserService } from "@/spi/AuthSPI";
import { UserService } from "@/infra/auth/UserService";
import AppLogger from "@/infra/logger/services/AppLogger";
import { IAddressService } from "@/spi/AddressSPI";
import AddressService from "@/services/AddressService";
import { NotificationService } from "@/infra/notification/NotificationService";
import AuthService from "@/infra/auth/AuthService";
import { ICustomerService } from "@/spi/CustomerSPI";
import CustomerServices from "@/customers/services/CustomerServices";
import { IAppSnackbarService } from "@/spi/AppSnackbarSPI";
import AppSnackbarService from "@/snackbar/services/AppSnackbarService";
import { IBackendClient } from "@/spi/BackendClientSPI";
import { CustomerBackendClientFacade } from "@/customers/services/CustomerBackendClientFacade";
import { Delivery } from "@/deliveries/models/Delivery";
import { DeliveryBackendClientFacade } from "@/deliveries/services/DeliveryBackendClientFacade";
import DeliveryStorageContext from "@/deliveries/services/DeliveryStorageContext";
import { IDeliveryService } from "@/spi/DeliverySPI";
import DeliveryServices from "@/deliveries/services/DeliveryServices";

export default class IOCContainerBootstrapper {
    public static init() {

        IOCContainerBootstrapper.bindUserService();

        IOCContainerBootstrapper.bindAuthService();

        IOCContainerBootstrapper.bindLogger();

        IOCContainerBootstrapper.bindNotificationHub();

        IOCContainerBootstrapper.bindCustomerStorageContext();

        IOCContainerBootstrapper.bindDeliveryStorageContext();

        IOCContainerBootstrapper.bindCustomerServices();

        IOCContainerBootstrapper.bindDeliveryServices();

        IOCContainerBootstrapper.bindAddressService();

        IOCContainerBootstrapper.bindAppSnackbarService();

        IOCContainerBootstrapper.bindCustomerBackendClientFacade();

        IOCContainerBootstrapper.bindDeliveryBackendClientFacade();

    }

    private static bindUserService() {
        IOCContainer.bind<IUserService>(ServicesIdentifiers.UserService).to(UserService).inSingletonScope();
    }

    private static bindAuthService() {
        IOCContainer.bind<IAuthService>(ServicesIdentifiers.AuthService).to(AuthService).inSingletonScope();
    }

    private static bindLogger() {
        const LOKI_HOST = process.env.EXPO_PUBLIC_LOKI_URL;
        if (!LOKI_HOST) {
            throw new Error('Missing Logger environment variable');
        }

        IOCContainer
            .bind<ILoggerConfig>(ServicesIdentifiers.LoggerConfig)
            .toConstantValue({
                host: LOKI_HOST,
                app: 'bike-sherpa',
                platform: `${Platform.OS} - ${Platform.Version}`,
            });

        IOCContainer
            .bind<ILogger>(ServicesIdentifiers.Logger)
            .to(AppLogger)
            .inSingletonScope();
    }

    private static bindNotificationHub() {
        IOCContainer.bind<INotificationService>(ServicesIdentifiers.NotificationService).to(NotificationService);
    }

    private static bindCustomerStorageContext() {
        IOCContainer.bind<IStorageContext<Customer>>(ServicesIdentifiers.CustomerStorage).to(CustomerStorageContext).inSingletonScope();
    }

    private static bindDeliveryStorageContext() {
        IOCContainer.bind<IStorageContext<Delivery>>(ServicesIdentifiers.DeliveryStorage).to(DeliveryStorageContext).inSingletonScope();
    }


    private static bindCustomerServices() {
        IOCContainer.bind<ICustomerService>(ServicesIdentifiers.CustomerServices).to(CustomerServices).inSingletonScope();
    }

    private static bindDeliveryServices() {
        IOCContainer.bind<IDeliveryService>(ServicesIdentifiers.DeliveryServices).to(DeliveryServices).inSingletonScope();
    }

    private static bindAddressService() {
        IOCContainer.bind<IAddressService>(ServicesIdentifiers.AddressService).to(AddressService).inSingletonScope();
    }

    private static bindAppSnackbarService() {
        IOCContainer.bind<IAppSnackbarService>(ServicesIdentifiers.AppSnackbarService).to(AppSnackbarService).inSingletonScope();
    }

    private static bindCustomerBackendClientFacade() {
        IOCContainer.bind<IBackendClient<Customer>>(ServicesIdentifiers.CustomerBackendClientFacade).to(CustomerBackendClientFacade).inSingletonScope();
    }

    private static bindDeliveryBackendClientFacade() {
        IOCContainer.bind<IBackendClient<Delivery>>(ServicesIdentifiers.DeliveryBackendClientFacade).to(DeliveryBackendClientFacade).inSingletonScope();
    }
}