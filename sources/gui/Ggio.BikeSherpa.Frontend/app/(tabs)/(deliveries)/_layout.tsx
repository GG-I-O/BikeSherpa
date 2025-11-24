import { Stack } from 'expo-router';
import { navigate } from "expo-router/build/global-state/routing";
import React from 'react';
import { Button, Text } from 'react-native-paper';

export default function DeliveryLayout() {
    return (
        <Stack>
            <Stack.Screen
                name="index"
                options={{
                    headerShown: true,
                    title: "Courses",
                    headerRight: () =>
                        <Button
                            style={{ marginRight: 8 }}
                            mode='outlined'
                            onPress={() => {
                                navigate({
                                    pathname: '/(tabs)/(deliveries)/new'
                                });
                            }}
                        >
                            <Text>Nouveau</Text>
                        </Button>
                }}
            />
            <Stack.Screen
                name="new"
                options={{
                    headerShown: true,
                    title: 'Nouvelle course'
                }}
            />
            <Stack.Screen
                name="copy"
                options={{
                    headerShown: true,
                    title: "Copier"
                }}
            />
            <Stack.Screen
                name="edit"
                options={{
                    headerShown: true,
                    title: "Modifier"
                }}
            />
            <Stack.Screen
                name="assign"
                options={{
                    headerShown: true,
                    title: "Détails"
                }}
            />
            <Stack.Screen
                name="[courseId]"
                options={{
                    headerShown: true,
                    title: "Détails"
                }}
            />
        </Stack>
    )
}