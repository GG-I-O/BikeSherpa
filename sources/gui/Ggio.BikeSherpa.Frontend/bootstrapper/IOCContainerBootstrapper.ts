import Customer from "@/customers/models/Customer";
import CustomerStorageContext from "@/customers/services/CustomerStorageContext";
// import { NotificationService } from "@/infra/storage/notification/NotificationService";
// import AppLogger from "@/logs/services/AppLogger";
import { ILogger, ILoggerConfig } from "@/spi/LogsSPI";
import { INotificationService, IStorageContext } from "@/spi/StorageSPI";
import { Platform } from "react-native";
import { IOCContainer } from "./constants/IOCContainer";
import { ServicesIndentifiers } from "./constants/ServicesIdentifiers";

export default class IOCContainerBootstrapper {
    public static init() {
        // IOCContainerBootstrapper.bindLogger();

        // IOCContainerBootstrapper.bindNotificationHub();

        IOCContainerBootstrapper.bindCustomerStorageContext();
    }

    // private static bindLogger() {
    //     const LOKI_HOST = process.env.EXPO_PUBLIC_LOKI_URL;
    //     if (!LOKI_HOST) {
    //         throw new Error('Missing Logger environment variable');
    //     }

    //     IOCContainer
    //         .bind<ILoggerConfig>(ServicesIndentifiers.LoggerConfig)
    //         .toConstantValue({
    //             host: LOKI_HOST,
    //             app: 'citysherpaPOC',
    //             platform: `${Platform.OS} - ${Platform.Version}`,
    //         });

    //     IOCContainer
    //         .bind<ILogger>(ServicesIndentifiers.Logger)
    //         .to(AppLogger)
    //         .inSingletonScope();
    // }

    // private static bindNotificationHub() {
    //     IOCContainer.bind<INotificationService>(ServicesIndentifiers.NotificationService).to(NotificationService);
    // }

    private static bindCustomerStorageContext() {
        IOCContainer.bind<IStorageContext<Customer>>(ServicesIndentifiers.CustomerStorage).to(CustomerStorageContext).inSingletonScope();
    }
}