import { useCallback } from "react";
import { Text, View } from "react-native";
import { useAuth0 } from "react-native-auth0";
import { Button } from "react-native-paper";

export default function Login() {
    const { authorize, error, isLoading } = useAuth0();
    const audience = process.env.EXPO_PUBLIC_AUTH_AUDIENCE;
    const scope = process.env.EXPO_PUBLIC_AUTH_DEV_SCOPE;

    const onLogin = useCallback(async () => {
        try {
            if (audience)
                await authorize({
                    audience,
                    scope
                });
        } catch (e) {
            console.error(e);
        }
    }, [authorize, audience, scope]);

    if (isLoading) {
        return (
            <View style={{ justifyContent: 'center', alignItems: 'center', height: '100%' }}>
                <Text>Loading</Text>
            </View>
        );
    }

    return (
        <View style={{ justifyContent: 'center', alignItems: 'center', height: '100%' }}>
            <Button onPress={onLogin}>
                <Text>Se Connecter</Text>
            </Button>
            {error && <Text>{error.message}</Text>}
        </View>
    );
}