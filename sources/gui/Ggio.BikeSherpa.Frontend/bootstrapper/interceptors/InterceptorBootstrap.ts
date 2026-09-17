import axios from 'axios';
import { IOCContainer } from '@/bootstrapper/constants/IOCContainer';
import { ServicesIdentifiers } from '@/bootstrapper/constants/ServicesIdentifiers';
import { ILogger } from '@/spi/LogsSPI';
import AuthInterceptor from '@/infra/auth/AuthInterceptor';
import { IAuthService } from '@/spi/AuthSPI';
import {AuthServiceIdentifier} from "@/infra/auth/bootstrapper/AuthServiceIdentifier";

export default class InterceptorBootstrap {

    private readonly logger: ILogger;

    public constructor() {
        this.logger = IOCContainer.get<ILogger>(ServicesIdentifiers.Logger);
        this.logger = this.logger.extend("Interceptors");
    }

    public startToIntercept() {
        axios.interceptors.request.use(
            (config) => {
                if (!config.headers['Content-Type']) {
                    config.headers['Content-Type'] = 'application/json';
                }

                return config;
            },
            (error) => {
                this.logger.error("Interceptors error :", error);
                return Promise.reject(error);
            }
        );

        // Authentification interceptor
        const authService = IOCContainer.get<IAuthService>(AuthServiceIdentifier.AuthService);
        const authInterceptor = new AuthInterceptor(this.logger, authService);
        authInterceptor.intercept();
    }
}