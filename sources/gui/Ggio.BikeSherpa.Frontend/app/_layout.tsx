import AppBootstrapper from "@/bootstrapper/AppBootstrapper";
import { IOCContainer } from "@/bootstrapper/constants/IOCContainer";
import { ServicesIdentifiers } from "@/bootstrapper/constants/ServicesIdentifiers";
import { IAuthService, IUserService } from "@/spi/AuthSPI";
import { Stack } from "expo-router";
import { useEffect } from "react";
import { Platform } from "react-native";
import { Auth0Provider, useAuth0 } from "react-native-auth0";

AppBootstrapper.init();

function AppStack() {
    const { user, getCredentials } = useAuth0();

    const loggedIn = user !== null && user !== undefined;

    const userService = IOCContainer.get<IUserService>(ServicesIdentifiers.UserService);
    const authService = IOCContainer.get<IAuthService>(ServicesIdentifiers.AuthService);
    useEffect(() => {
        userService.setCurrentUser(user);
        authService.setCredentialMethod(getCredentials);
    }, [userService, user, getCredentials]);

    return (
        <Stack>
            <Stack.Protected guard={loggedIn}>
                <Stack.Screen name="(tabs)" options={{ headerShown: false }} />
            </Stack.Protected>
            <Stack.Protected guard={!loggedIn}>
                <Stack.Screen name="login" options={{ headerShown: false }} />
            </Stack.Protected>
        </Stack>
    );
}

export default function RootLayout() {
    const authDomain = process.env.EXPO_PUBLIC_AUTH_DOMAIN;

    let authClient;
    if (Platform.OS === "android")
        authClient = process.env.EXPO_PUBLIC_AUTH_CLIENT_ANDROID;
    else
        authClient = process.env.EXPO_PUBLIC_AUTH_CLIENT_WEB;

    return (
        <Auth0Provider domain={authDomain ?? ''} clientId={authClient ?? ''}>
            <AppStack />
        </Auth0Provider>
    );
}