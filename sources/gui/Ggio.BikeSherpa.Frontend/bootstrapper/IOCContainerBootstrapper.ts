import Customer from "@/customers/models/Customer";
import CustomerStorageContext from "@/customers/services/CustomerStorageContext";
// import { NotificationService } from "@/infra/storage/notification/NotificationService";
// import AppLogger from "@/logs/services/AppLogger";
import { ILogger, ILoggerConfig } from "@/spi/LogsSPI";
import { INotificationService, IStorageContext } from "@/spi/StorageSPI";
import { Platform } from "react-native";
import { IOCContainer } from "./constants/IOCContainer";
import { ServicesIndentifiers } from "./constants/ServicesIdentifiers";
import { IUserService } from "@/spi/AuthSPI";
import { UserService } from "@/infra/auth/UserService";
import AppLogger from "@/infra/logger/services/AppLogger";

export default class IOCContainerBootstrapper {
    public static init() {

        IOCContainerBootstrapper.bindUserService();

        IOCContainerBootstrapper.bindLogger();

        // IOCContainerBootstrapper.bindNotificationHub();

        IOCContainerBootstrapper.bindCustomerStorageContext();
    }

    private static bindUserService() {
        IOCContainer.bind<IUserService>(ServicesIndentifiers.UserService).to(UserService).inSingletonScope();
    }

    private static bindLogger() {
        const LOKI_HOST = process.env.EXPO_PUBLIC_LOKI_URL;
        if (!LOKI_HOST) {
            throw new Error('Missing Logger environment variable');
        }

        IOCContainer
            .bind<ILoggerConfig>(ServicesIndentifiers.LoggerConfig)
            .toConstantValue({
                host: LOKI_HOST,
                app: 'bike-sherpa',
                platform: `${Platform.OS} - ${Platform.Version}`,
            });

        IOCContainer
            .bind<ILogger>(ServicesIndentifiers.Logger)
            .to(AppLogger)
            .inSingletonScope();
    }

    // private static bindNotificationHub() {
    //     IOCContainer.bind<INotificationService>(ServicesIndentifiers.NotificationService).to(NotificationService);
    // }

    private static bindCustomerStorageContext() {
        IOCContainer.bind<IStorageContext<Customer>>(ServicesIndentifiers.CustomerStorage).to(CustomerStorageContext).inSingletonScope();
    }
}