import {Platform} from "react-native";

export default class Auth0ProviderOptions {
    private static readonly authDomain = process.env.EXPO_PUBLIC_AUTH_DOMAIN ?? "";
    private static readonly authClient = (Platform.OS === "android" ?
        process.env.EXPO_PUBLIC_AUTH_ANDROID_CLIENT :
        process.env.EXPO_PUBLIC_AUTH_WEB_CLIENT) ?? "";
    
    public static getAuth0ProviderOptions(useDPoP: boolean) {
        return {
            domain: Auth0ProviderOptions.authDomain,
            clientId: Auth0ProviderOptions.authClient,
            useDPoP
        }
    }
}