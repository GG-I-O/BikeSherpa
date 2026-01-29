import { Stack } from 'expo-router';
import { navigate } from 'expo-router/build/global-state/routing';
import React from 'react';
import { Button, Text } from 'react-native-paper';

export default function CourierLayout() {
    return (
        <Stack>
            <Stack.Screen
                name="index"
                options={{
                    headerShown: true,
                    title: "Livreurs",
                    headerRight: () =>
                        <Button
                            style={{ marginRight: 8 }}
                            mode='outlined'
                            onPress={() => {
                                navigate({
                                    pathname: '/(tabs)/(couriers)/new'
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
                    title: "Nouveau livreur"
                }}
            />
            <Stack.Screen
                name="edit"
                options={{
                    headerShown: true,
                    title: "Modifier le livreur"
                }}
            />
        </Stack>
    )
}