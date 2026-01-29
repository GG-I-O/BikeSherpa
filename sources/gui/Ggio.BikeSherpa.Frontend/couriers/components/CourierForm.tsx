import ThemedAddressInput from "@/components/themed/ThemedAddressInput";
import ThemedInput from "@/components/themed/ThemedInput";
import formStyle from "@/style/formStyle";
import { Control, FieldError, FieldErrors, FieldValues } from "react-hook-form";
import { ScrollView } from "react-native";
import { Button, Text, useTheme } from "react-native-paper";
import React from 'react'

interface CourierFormProps<T extends FieldValues> {
    control: Control<T, any, T>;
    handleSubmit: (e?: React.BaseSyntheticEvent) => Promise<void>;
    errors: FieldErrors<T>;
    buttonName: string;
}

export default function CourierForm<T extends FieldValues>(props: CourierFormProps<T>) {
    const theme = useTheme();

    const { control, errors, handleSubmit, buttonName } = props;

    return (
        <ScrollView
            style={[formStyle.container, { backgroundColor: theme.colors.background }]}
            contentContainerStyle={formStyle.elements}
        >
            <ThemedInput
                testID="courierFormFirstNameInput"
                control={control}
                name="firstName"
                error={errors.firstName as FieldError | undefined}
                label="Prénom"
                placeholder="Jean-Claude"
                required
            />
            <ThemedInput
                testID="courierFormLastNameInput"
                control={control}
                name="lastName"
                error={errors.firstName as FieldError | undefined}
                label="Nom"
                placeholder="Dusse"
                required
            />
            <ThemedInput
                testID="courierFormCodeInput"
                control={control}
                name="code"
                error={errors.code as FieldError | undefined}
                label="Code"
                placeholder="JCD"
                required
            />
            <ThemedInput
                testID="courierFormEmailInput"
                control={control}
                name="email"
                error={errors.email as FieldError | undefined}
                label="E-mail"
                placeholder="jean-claude-dusse@gmail.fr"
                required
            />
            <ThemedInput
                testID="courierFormPhoneInput"
                control={control}
                name="phoneNumber"
                error={errors.phoneNumber as FieldError | undefined}
                label="Téléphone"
                placeholder="06 10 11 12 13"
                required
            />
            <ThemedAddressInput
                control={control}
                name="address"
                error={(errors.address as any)?.name as FieldError | undefined}
                label="Adresse"
                placeholder="10 rue de la République 38100 Grenoble"
                required
            />
            <ThemedInput
                testID="courierFormComplementInput"
                control={control}
                name="complement"
                error={(errors.address as any)?.complement as FieldError | undefined}
                label="Complément d’adresse"
                placeholder="Bâtiment B"
            />
            <Button
                testID="formButton"
                mode="outlined"
                onPress={() => handleSubmit()}
                style={formStyle.button}
            >
                <Text testID="buttonName">{buttonName}</Text>
            </Button>
        </ScrollView>
    );
}