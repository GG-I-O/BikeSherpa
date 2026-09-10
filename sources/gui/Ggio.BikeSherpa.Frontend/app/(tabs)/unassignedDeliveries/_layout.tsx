import { Stack } from 'expo-router';
import React from 'react';

export default function UnassignedDeliveriesLayout() {
    return (
        <Stack>
            <Stack.Screen
                name="index"
                options={{
                    headerShown: true,
                    title: 'Non assignées'
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
