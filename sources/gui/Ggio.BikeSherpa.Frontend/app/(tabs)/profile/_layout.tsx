import {Stack} from "expo-router";
import React from "react";

export default function ProfileLayout() {
    return (
        <Stack>
            <Stack.Screen
                name="index"
                options={{
                    headerShown: true,
                    title: 'Mon profil'
                }}
            />
        </Stack>
    )
}