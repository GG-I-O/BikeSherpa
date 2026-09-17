import { ServicesIdentifiers } from "@/bootstrapper/constants/ServicesIdentifiers";
import { IAuthService } from "@/spi/AuthSPI";
import { ILogger } from "@/spi/LogsSPI";
import axios from "axios";
import { inject } from "inversify";
import {AuthServiceIdentifier} from "@/infra/auth/bootstrapper/AuthServiceIdentifier";

export default class AuthInterceptor {
    private logger: ILogger;
    private authService: IAuthService;

    public constructor(
        @inject(ServicesIdentifiers.Logger) logger: ILogger,
        @inject(AuthServiceIdentifier.AuthService) authService: IAuthService
    ) {
        this.logger = logger.extend("AuthInterceptor");
        this.authService = authService;
    }

    public intercept() {
        // Request interceptor - Add Bearer token
        axios.interceptors.request.use(
            async (config) => {
                const token = await this.authService.getToken();
                if (token)
                    config.headers.Authorization = 'Bearer ' + token;
                return config;
            },
            (error) => {
                this.logger.error("AuthInterceptor error :", error);
                return Promise.reject(error);
            }
        );
    }
}