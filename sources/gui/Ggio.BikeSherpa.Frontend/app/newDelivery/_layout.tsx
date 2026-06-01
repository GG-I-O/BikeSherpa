import {Stack} from "expo-router";
import React from "react";

export default function PublicDeliveryLayout() {
    return (
        <Stack>
            <Stack.Screen
                name="index"
                options={{
                    headerShown: true,
                    title: 'Connexion'
                }}
            />
            <Stack.Screen
                name="form"
                options={{
                    headerShown: true,
                    title: 'Nouvelle course'
                }}
            />
            <Stack.Screen
                name="summary"
                options={{
                    headerShown: true,
                    title: 'Récapitulatif'
                }}
            />
        </Stack>
    );
}