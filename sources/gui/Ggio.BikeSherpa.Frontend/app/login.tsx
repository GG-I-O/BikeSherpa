import { Text, View } from "react-native";
import { useAuth0 } from "react-native-auth0";
import { Button } from "react-native-paper";

export default function Login() {

    const { authorize, error, isLoading } = useAuth0();

    const onLogin = async () => {
        try {
            await authorize({
                scope: 'openid profile email offline_access'
            });
        } catch (e) {
            console.error(e);
        }
    };

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