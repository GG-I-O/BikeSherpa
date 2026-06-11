import { Stack } from 'expo-router';
import React from 'react';

export default function ReportLayout() {
    return (
        <Stack>
            <Stack.Screen
                name="index"
                options={{
                    headerShown: true,
                    title: 'Mes rapports'
                }}
            />
        </Stack>
    )
}
