import { Stack } from 'expo-router';
import React from 'react';

export default function MyDeliveryLayout() {
    return (
        <Stack>
            <Stack.Screen
                name="index"
                options={{
                    headerShown: true,
                    title: 'Mes courses'
                }}
            />
            <Stack.Screen
                name="[stepId]"
                options={{
                    headerShown: true,
                    title: 'Détails'
                }}
            />
        </Stack>
    )
}
