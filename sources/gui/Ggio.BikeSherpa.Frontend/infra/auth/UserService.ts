import {IUserService, UserLogInfo} from '@/spi/AuthSPI';
import {injectable} from 'inversify';
import {User} from 'react-native-auth0';
import {createApiClient} from "@/infra/openAPI/client";
import axios from "axios";

/**
 * Singleton used for IOC
 */
@injectable()
export class UserService implements IUserService {
    private currentUser: User | null = null;
    private currentUserLogInfo: UserLogInfo | null = null;
    private apiClient;

    constructor() {
        this.apiClient = createApiClient(axios.defaults.baseURL || '', {
            axiosInstance: axios
        });
    }

    /**
     * Set the current authenticated user
     * @param user - User for logged in user
     */
    public setCurrentUser(user: User | null): void {
        this.currentUser = user;
    }

    /**
     * Get the current authenticated user profile
     * @returns User if logged in, null if logged out
     */
    public getUserProfile(): User | null {
        return this.currentUser;
    }

    public setUserLogInfo(isDispatcher: boolean): void {
        if (!isDispatcher) {
            if (this.currentUser !== null) {
                try {
                    // if user is courier, it will give the profile    
                    this.apiClient.GetCourierMyselfEndpoint().then(
                        (courier) => {
                            this.currentUserLogInfo = {
                                id: courier.data.id
                            }
                        }
                    )
                } catch (e) {
                    // if user is not courier, it will give a 404
                }
            }
        }
    }

    /**
     * Get the current authenticated user for logger
     * @returns UserLogInfo if logged in, null if logged out
     */
    getUserLogInfo(): UserLogInfo | null {
        return this.currentUserLogInfo;
    }
}