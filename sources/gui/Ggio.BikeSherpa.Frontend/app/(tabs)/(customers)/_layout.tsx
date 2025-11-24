import { Stack } from 'expo-router';
import { navigate } from 'expo-router/build/global-state/routing';
import React from 'react';
import { Button, Text } from 'react-native-paper';

export default function CustomerLayout() {
    return (
        <Stack>
            <Stack.Screen
                name="index"
                options={{
                    headerShown: true,
                    title: "Clients",
                    headerRight: () =>
                        <Button
                            style={{ marginRight: 8 }}
                            mode='outlined'
                            onPress={() => {
                                navigate({
                                    pathname: '/(tabs)/(customers)/new'
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
                    title: "Nouveau client"
                }}
            />
        </Stack>
    )
}