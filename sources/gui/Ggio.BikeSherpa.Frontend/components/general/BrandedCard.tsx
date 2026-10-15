import React from "react";
import {Image, StyleSheet, View, ViewStyle, StyleProp} from "react-native";
import {Card, Text, useTheme} from "react-native-paper";
import {KeyboardAwareScrollView} from "react-native-keyboard-controller";

type BrandedCardProps = {
    title?: string;
    subtitle?: string;
    footer?: string | null;
    children?: React.ReactNode;
    style?: StyleProp<ViewStyle>;
};

export default function BrandedCard(
    {
        title = "Bike Sherpa",
        subtitle = "Prêt pour votre prochaine course ?",
        footer = "Propulsé par Bike Sherpa Logistics",
        children,
        style,
    }: BrandedCardProps) {
    const theme = useTheme();

    return (
        <KeyboardAwareScrollView
            contentContainerStyle={[styles.container, {backgroundColor: theme.colors.background}, style]}
        >
            <Card style={styles.card}>
                <Card.Content style={styles.content}>
                    <View style={styles.logoContainer}>
                        <Image
                            source={require("../../assets/images/icon.png")}
                            style={styles.logo}
                        />
                    </View>
                    <Text variant="headlineMedium" style={styles.title}>
                        {title}
                    </Text>
                    <Text variant="bodyLarge" style={styles.subtitle}>
                        {subtitle}
                    </Text>

                    <View style={styles.body}>{children}</View>
                </Card.Content>
            </Card>
            {footer ? <Text style={styles.footer}>{footer}</Text> : null}
        </KeyboardAwareScrollView>
    );
}

const styles = StyleSheet.create({
    container: {
        flex: 1,
        justifyContent: "center",
        alignItems: "center",
        padding: 20,
    },
    card: {
        width: "100%",
        maxWidth: 400,
        elevation: 4,
        borderRadius: 12,
    },
    content: {
        alignItems: "center",
        paddingVertical: 30,
    },
    logoContainer: {
        marginBottom: 20,
        backgroundColor: "#f0f0f0",
        padding: 15,
        borderRadius: 50,
    },
    logo: {
        width: 80,
        height: 80,
        resizeMode: "contain",
    },
    title: {
        fontWeight: "bold",
        marginBottom: 5,
        color: "#2e7d32",
    },
    subtitle: {
        textAlign: "center",
        marginBottom: 30,
        opacity: 0.7,
    },
    body: {
        width: "100%"
    },
    footer: {
        position: "absolute",
        bottom: 30,
        opacity: 0.5,
        fontSize: 12,
    },
});