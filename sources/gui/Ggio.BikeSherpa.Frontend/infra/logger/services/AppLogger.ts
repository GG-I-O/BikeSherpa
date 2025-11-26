import { consoleTransport, logger } from "react-native-logs";
import { createLokiTransport } from "../options/LokiTransporter";
import { ServicesIndentifiers } from "@/bootstrapper/constants/ServicesIdentifiers";
import { ILogger, ILoggerConfig } from "@/spi/LogsSPI";
import { IUserService } from "@/spi/AuthSPI";
import { inject, injectable } from "inversify";

@injectable()
export default class AppLogger implements ILogger {
    private logger: any;
    private userService: IUserService;

    public constructor(
        @inject(ServicesIndentifiers.UserService) userService: IUserService,
        @inject(ServicesIndentifiers.LoggerConfig) config: ILoggerConfig
    ) {
        this.userService = userService;

        // Create Loki transport with batching for better performance
        const lokiTransport = createLokiTransport({
            host: config.host,
            labels: {
                app: config.app,
                platform: config.platform,
                user: () => {
                    const user = this.userService.getUserLogInfo();
                    return user?.name ? user.name : 'anonymous';
                }
            },
            batching: {
                enabled: true,
                interval: 5000, // Send batch every 5 seconds
                maxBatchSize: 10, // Or when 10 logs accumulate
            },
        });

        this.logger = logger.createLogger({
            levels: {
                debug: 0,
                info: 1,
                warn: 2,
                error: 3,
            },
            // Use multiple transports: console for debugging + Loki for storage
            transport: __DEV__ ? [consoleTransport, lokiTransport] : lokiTransport,
            severity: __DEV__ ? "debug" : "error",
            transportOptions: {
                colors: {
                    info: "blueBright",
                    warn: "yellowBright",
                    error: "redBright",
                },
            },
            async: true,
            dateFormat: "iso",
            printLevel: true,
            printDate: true,
            fixedExtLvlLength: false,
            enabled: true,
        });
    }

    error(...args: unknown[]): void {
        this.logger.error(...args);
    }

    warn(...args: unknown[]): void {
        this.logger.warn(...args);
    }

    info(...args: unknown[]): void {
        this.logger.info(...args);
    }

    debug(...args: unknown[]): void {
        this.logger.debug(...args);
    }

    /**
     * Create a logger with a namespace
     * 
     * Example: const authLogger = logger.extend('AUTH');
     *          authLogger.info('Login successful'); // [AUTH] Login successful
     */
    extend(extension: string): ILogger {
        const extendedLogger = this.logger.extend(extension);

        // Return a new ILogger that wraps the extended logger
        return {
            error: (...args: unknown[]) => extendedLogger.error(...args),
            warn: (...args: unknown[]) => extendedLogger.warn(...args),
            info: (...args: unknown[]) => extendedLogger.info(...args),
            debug: (...args: unknown[]) => extendedLogger.debug(...args),
            extend: (ext: string) => this.extend(`${extension}.${ext}`),
        };
    }
}