import AppBootstrapper from "@/bootstrapper/AppBootstrapper";
import { Stack } from "expo-router";
import { Platform } from "react-native";
import { Auth0Provider, useAuth0 } from "react-native-auth0";

AppBootstrapper.init();

function AppStack() {
    const { user } = useAuth0();

    const loggedIn = user !== undefined && user !== null;

    return (
        <Stack>
            <Stack.Protected guard={loggedIn}>
                <Stack.Screen name="(tabs)" options={{ headerShown: false }} />
            </Stack.Protected>
            <Stack.Protected guard={!loggedIn}>
                <Stack.Screen name='login' options={{ headerShown: false }} />
            </Stack.Protected>
        </Stack>
    );
}

export default function RootLayout() {
    const authDomain = process.env.EXPO_PUBLIC_AUTH_DOMAIN;
    let authClient;
    if (Platform.OS == 'android')
        authClient = process.env.EXPO_PUBLIC_AUTH_CLIENT_ANDROID;
    else
        authClient = process.env.EXPO_PUBLIC_AUTH_CLIENT_WEB;

    return (
        <Auth0Provider domain={authDomain ?? ''} clientId={authClient ?? ''}>
            <AppStack />
        </Auth0Provider>
    );

}