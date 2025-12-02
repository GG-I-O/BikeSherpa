import axios from 'axios';
// import AuthInterceptor from '../../auth/services/AuthInterceptor';
import { IOCContainer } from '@/bootstrapper/constants/IOCContainer';
// import { IAuthExtension } from '@/auth/services/AuthExtension';
import { ServicesIndentifiers } from '@/bootstrapper/constants/ServicesIdentifiers';
import { ILogger } from '@/spi/LogsSPI';

export default class Interceptors {

    private logger: ILogger;

    public constructor() {
        axios.defaults.baseURL = process.env.EXPO_PUBLIC_API_URL;

        // Logger
        this.logger = IOCContainer.get<ILogger>(ServicesIndentifiers.Logger);
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
    //     const authExtension = IOCContainer.get<IAuthExtension>(ServicesIndentifiers.AuthService);
    //     const authInterceptor = new AuthInterceptor(authExtension, this.logger);
    //     authInterceptor.intercept();
    }
}