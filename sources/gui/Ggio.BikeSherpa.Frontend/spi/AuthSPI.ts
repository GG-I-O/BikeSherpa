import {User} from "react-native-auth0";

export interface UserLogInfo {
    id: string;
}

export interface IUserService {
    getUserLogInfo: () => UserLogInfo | null;
    getUserProfile: () => User | null;
    setCurrentUser: (user: User | null) => void;
    setUserLogInfo: (isCourier: boolean) => void;
}

export interface IAuthService {
    setCredentialMethod: (method: (params: any) => Promise<any>) => void;
    getToken: () => Promise<string | null>;
    isDispatcher: () => Promise<boolean>;
}

export interface IUsernameService {
    setUsername: (username: string) => void;
    getUsername: () => string;
}