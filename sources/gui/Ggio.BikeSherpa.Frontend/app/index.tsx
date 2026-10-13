import {useCallback} from "react";
import {StyleSheet, View} from "react-native";
import {useAuth0} from "react-native-auth0";
import {Button, Text, ActivityIndicator, useTheme} from "react-native-paper";
import BrandedCard from "@/components/general/BrandedCard";

export default function Login() {
    const {authorize, error, isLoading} = useAuth0();
    const audience = process.env.EXPO_PUBLIC_AUTH_AUDIENCE;
    const scope = process.env.EXPO_PUBLIC_AUTH_SCOPE;
    const theme = useTheme();

    const onLogin = useCallback(async () => {
        try {
            if (audience)
                await authorize({audience, scope});
        } catch (e) {
            console.error(e);
        }
    }, [authorize, audience, scope]);

    if (isLoading) {
        return (
            <View style={styles.loadingContainer}>
                <ActivityIndicator animating={true} size="large"/>
                <Text style={styles.loadingText}>Chargement en cours...</Text>
            </View>
        );
    }

    return (
        <BrandedCard>
            <Button
                mode="contained"
                onPress={onLogin}
                style={styles.button}
                contentStyle={styles.buttonContent}
            >
                Se Connecter
            </Button>

            {error && (
                <Text style={[styles.error, {color: theme.colors.error}]}>
                    {error.message}
                </Text>
            )}
        </BrandedCard>
    );
}

const styles = StyleSheet.create({
    loadingContainer: {
        flex: 1,
        justifyContent: "center",
        alignItems: "center",
    },
    loadingText: {
        marginTop: 10,
    },
    button: {
        width: "100%",
        borderRadius: 8,
    },
    buttonContent: {
        paddingVertical: 6,
    },
    error: {
        textAlign: "center",
    },
});