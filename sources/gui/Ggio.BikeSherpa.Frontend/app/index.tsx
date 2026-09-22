import { useCallback } from "react";
import { StyleSheet, View, Image } from "react-native";
import { useAuth0 } from "react-native-auth0";
import { Button, Text, Card, ActivityIndicator, useTheme } from "react-native-paper";
import {IOCContainer} from "@/bootstrapper/constants/IOCContainer";
import {ILogger} from "@/spi/LogsSPI";
import {ServicesIdentifiers} from "@/bootstrapper/constants/ServicesIdentifiers";
import {authFallbackStore$} from "@/infra/auth/store/authFallbackStore";

export default function Login() {
    const { authorize, error, isLoading } = useAuth0();
    const audience = process.env.EXPO_PUBLIC_AUTH_AUDIENCE;
    const scope = process.env.EXPO_PUBLIC_AUTH_SCOPE;
    const theme = useTheme();

    let logger = IOCContainer.get<ILogger>(ServicesIdentifiers.Logger);
    logger = logger.extend("Login");

    const onLogin = useCallback(async () => {
        try {
            if (audience)
                await authorize({
                    audience,
                    scope
                });
        } catch (e) {
            const error = e as Error;
            const isDpopKeystoreFailure =
                error?.message?.includes('DPoP key pair') ||
                error?.message?.includes('SecureCredentialsManager');

            logger.error('authorize error ', { name: 'Login', message: e });

            if (isDpopKeystoreFailure) {
                authFallbackStore$.dpopDisabled.set(true);
            }
        }
    }, [authorize, audience, scope]);

    if (isLoading) {
        return (
            <View style={styles.loadingContainer}>
                <ActivityIndicator animating={true} size="large" />
                <Text style={styles.loadingText}>Chargement en cours...</Text>
            </View>
        );
    }

    return (
        <View style={[styles.container, { backgroundColor: theme.colors.background }]}>
            <Card style={styles.card}>
                <Card.Content style={styles.content}>
                    <View style={styles.logoContainer}>
                        <Image 
                            source={require('../assets/images/icon.png')} 
                            style={styles.logo} 
                        />
                    </View>
                    <Text variant="headlineMedium" style={styles.title}>Bike Sherpa</Text>
                    <Text variant="bodyLarge" style={styles.subtitle}>
                        Prêt pour votre prochaine course ?
                    </Text>
                    
                    <Button 
                        mode="contained" 
                        onPress={onLogin} 
                        style={styles.button}
                        contentStyle={styles.buttonContent}
                    >
                        Se Connecter
                    </Button>

                    {error && (
                        <Text style={[styles.error, { color: theme.colors.error }]}>
                            {error.message}
                        </Text>
                    )}
                </Card.Content>
            </Card>
            <Text style={styles.footer}>Propulsé par Bike Sherpa Logistics</Text>
        </View>
    );
}

const styles = StyleSheet.create({
    container: {
        flex: 1,
        justifyContent: 'center',
        alignItems: 'center',
        padding: 20,
    },
    loadingContainer: {
        flex: 1,
        justifyContent: 'center',
        alignItems: 'center',
    },
    loadingText: {
        marginTop: 10,
    },
    card: {
        width: '100%',
        maxWidth: 400,
        elevation: 4,
        borderRadius: 12,
    },
    content: {
        alignItems: 'center',
        paddingVertical: 30,
    },
    logoContainer: {
        marginBottom: 20,
        backgroundColor: '#f0f0f0',
        padding: 15,
        borderRadius: 50,
    },
    logo: {
        width: 80,
        height: 80,
        resizeMode: 'contain',
    },
    title: {
        fontWeight: 'bold',
        marginBottom: 5,
        color: '#2e7d32', // Un vert rappelant le vélo/écologie
    },
    subtitle: {
        textAlign: 'center',
        marginBottom: 30,
        opacity: 0.7,
    },
    button: {
        width: '100%',
        borderRadius: 8,
    },
    buttonContent: {
        paddingVertical: 6,
    },
    error: {
        marginTop: 15,
        textAlign: 'center',
    },
    footer: {
        position: 'absolute',
        bottom: 30,
        opacity: 0.5,
        fontSize: 12,
    }
});