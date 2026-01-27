import ThemedAddressInput from "@/components/themed/ThemedAddressInput";
import ThemedInput from "@/components/themed/ThemedInput";
import formStyle from "@/style/formStyle";
import { Control, FieldError, FieldErrors, FieldValues } from "react-hook-form";
import { ScrollView } from "react-native";
import { Button, Text, useTheme } from "react-native-paper";
import React from 'react'

interface CustomerFormProps<T extends FieldValues> {
    control: Control<T, any, T>;
    handleSubmit: (e?: React.BaseSyntheticEvent) => Promise<void>;
    errors: FieldErrors<T>;
    buttonName: string;
}

export default function CustomerForm<T extends FieldValues>(props: CustomerFormProps<T>) {
    const theme = useTheme();

    const { control, errors, handleSubmit, buttonName } = props;

    return (
        <ScrollView
            style={[formStyle.container, { backgroundColor: theme.colors.background }]}
            contentContainerStyle={formStyle.elements}
        >
            <ThemedInput
                testID="customerFormNameInput"
                control={control}
                name="name"
                error={errors.name as FieldError | undefined}
                label="Nom"
                placeholder="Ma Petite Société"
                required
            />
            <ThemedInput
                testID="customerFormCodeInput"
                control={control}
                name="code"
                error={errors.code as FieldError | undefined}
                label="Code"
                placeholder="MPS"
                required
            />
            <ThemedInput
                testID="customerFormEmailInput"
                control={control}
                name="email"
                error={errors.email as FieldError | undefined}
                label="E-mail"
                placeholder="votre-nom@societe.fr"
                required
            />
            <ThemedInput
                testID="customerFormPhoneInput"
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
                testID="customerFormComplementInput"
                control={control}
                name="complement"
                error={(errors.address as any)?.complement as FieldError | undefined}
                label="Complément d'adresse"
                placeholder="Bâtiment B"
            />
            <ThemedInput
                testID="customerFormSiretInput"
                control={control}
                name="siret"
                error={errors.siret as FieldError | undefined}
                label="Siret"
                placeholder="12345678910123"
            />
            <ThemedInput
                testID="customerFormVatNumberInput"
                control={control}
                name="vatNumber"
                error={errors.vatNumber as FieldError | undefined}
                label="Numéro de TVA"
                placeholder="FR12345678910"
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