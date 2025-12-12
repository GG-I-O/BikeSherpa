import axios from 'axios';
import { IOCContainer } from '@/bootstrapper/constants/IOCContainer';
import { ServicesIdentifiers } from '@/bootstrapper/constants/ServicesIdentifiers';
import { ILogger } from '@/spi/LogsSPI';
import AuthInterceptor from '@/infra/auth/AuthInterceptor';
import { IAuthService } from '@/spi/AuthSPI';

export default class InterceptorBootstrap {

    private logger: ILogger;

    public constructor() {
        this.logger = IOCContainer.get<ILogger>(ServicesIdentifiers.Logger);
        this.logger = this.logger.extend("Interceptors");
    }

    public startToIntercept() {
        axios.interceptors.request.use(
            (config) => {
                config.headers['Content-Type'] = 'application/json';
                return config;
            },
            (error) => {
                this.logger.error("Interceptors error :", error);
                return Promise.reject(error);
            }
        );

        // Authentification interceptor
        const authService = IOCContainer.get<IAuthService>(ServicesIdentifiers.AuthService);
        const authInterceptor = new AuthInterceptor(this.logger, authService);
        authInterceptor.intercept();
    }
}