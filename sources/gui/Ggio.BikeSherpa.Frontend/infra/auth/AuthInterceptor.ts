

import { ServicesIdentifiers } from "@/bootstrapper/constants/ServicesIdentifiers";
import { IAuthService } from "@/spi/AuthSPI";
import { ILogger } from "@/spi/LogsSPI";
import axios, { AxiosError, InternalAxiosRequestConfig } from "axios";
import { inject } from "inversify";

export default class AuthInterceptor {
    private logger: ILogger;
    private authService: IAuthService;

    private isRefreshing: boolean = false;
    private failedQueue: Array<{
        resolve: (value?: any) => void;
        reject: (error?: any) => void;
    }> = [];

    public constructor(
        @inject(ServicesIdentifiers.Logger) logger: ILogger,
        @inject(ServicesIdentifiers.AuthService) authService: IAuthService
    ) {
        this.logger = logger.extend("AuthInterceptor");
        this.authService = authService;
    }

    // private processQueue(error: any = null, token: string | null = null) {
    //     this.failedQueue.forEach(promise => {
    //         if (error) {
    //             promise.reject(error);
    //         } else {
    //             promise.resolve(token);
    //         }
    //     });
    //     this.failedQueue = [];
    // }

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

        // Response interceptor - Handle 401 and refresh token
        // axios.interceptors.response.use(
        //     (response) => {
        //         // If response is successful, just return it
        //         return response;
        //     },
        //     async (error: AxiosError) => {
        //         const originalRequest = error.config as InternalAxiosRequestConfig & { _retry?: boolean };

        //         // Check if error is 401 and we haven't already retried
        //         if (error.response?.status === 401 && !originalRequest._retry) {
        //             // If we're already refreshing, queue this request
        //             if (this.isRefreshing) {
        //                 return new Promise((resolve, reject) => {
        //                     this.failedQueue.push({ resolve, reject });
        //                 })
        //                     .then(() => {
        //                         // Retry the original request with new token
        //                         const tokens = this.authExtension.getTokens();
        //                         if (tokens?.accessToken) {
        //                             originalRequest.headers.Authorization = 'Bearer ' + tokens.accessToken;
        //                         }
        //                         return axios(originalRequest);
        //                     })
        //                     .catch(err => {
        //                         return Promise.reject(err);
        //                     });
        //             }

        //             // Mark that we're attempting to retry
        //             originalRequest._retry = true;
        //             this.isRefreshing = true;

        //             try {
        //                 this.logger.info("Token expired (401), attempting refresh...");

        //                 // Attempt to refresh the token
        //                 await this.authExtension.refresh();

        //                 const tokens = this.authExtension.getTokens();

        //                 if (!tokens?.accessToken) {
        //                     throw new Error("No access token after refresh");
        //                 }

        //                 this.logger.info("Token refresh successful");

        //                 // Update the failed queue with the new token
        //                 this.processQueue(null, tokens.accessToken);

        //                 // Retry the original request with new token
        //                 originalRequest.headers.Authorization = 'Bearer ' + tokens.accessToken;
        //                 return axios(originalRequest);

        //             } catch (refreshError) {
        //                 this.logger.error("Token refresh failed:", refreshError);

        //                 // Process queue with error (all queued requests will fail)
        //                 this.processQueue(refreshError, null);

        //                 // User will be logged out by the refresh() method
        //                 return Promise.reject(refreshError);
        //             } finally {
        //                 this.isRefreshing = false;
        //             }
        //         }

        //         // For other errors or if retry failed, reject
        //         this.logger.error("Response error:", error.response?.status, error.message);
        //         return Promise.reject(error);
        //     }
        // );
    }
}