import { User } from "react-native-auth0";

export interface UserLogInfo {
    name?: string;
    email?: string;
}

export interface IUserService {
    getUserLogInfo: () => UserLogInfo | null;
    getUserProfile: () => User | null;
    setCurrentUser: (user: User | null) => void;
}