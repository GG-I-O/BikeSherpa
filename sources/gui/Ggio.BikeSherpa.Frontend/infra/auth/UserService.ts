import { IUserService, UserLogInfo } from '@/spi/AuthSPI';
import { injectable } from 'inversify';
import { User } from 'react-native-auth0';

/**
 * Singleton used for IOC
 */
@injectable()
export class UserService implements IUserService {
    private currentUser: User | null = null;

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

    /**
     * Get the current authenticated user for logger
     * @returns UserLogInfo if logged in, null if logged out
     */
    getUserLogInfo(): UserLogInfo | null {
        return this.currentUser as UserLogInfo;
    }

    
}