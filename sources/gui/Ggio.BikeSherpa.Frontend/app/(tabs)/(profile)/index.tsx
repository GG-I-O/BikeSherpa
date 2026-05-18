import {Text, View} from "react-native";
import {Button} from "react-native-paper";
import {useAuth0} from "react-native-auth0";

export default function Profile() {
    const { clearSession } = useAuth0();
    return (
        <View style={{ justifyContent: 'center', alignItems: 'center', height: '100%' }}>
            <Button onPress={() => clearSession()}>
                <Text>Déconnexion</Text>
            </Button>
        </View>
    );
}