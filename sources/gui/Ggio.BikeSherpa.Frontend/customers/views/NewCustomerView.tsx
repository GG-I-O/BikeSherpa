import formStyle from "@/style/formStyle";
import { useState } from "react";
import { ScrollView } from "react-native";
import { Button, Checkbox, Text, TextInput, useTheme } from "react-native-paper";
import useCustomerViewModel from "../viewModel/CustomerViewModel";
import ThemedInput from "@/components/themed/ThemedInput";
import ThemedCheckbox from "@/components/themed/ThemedCheckbox";

export default function NewCustomerView() {
    const theme = useTheme();

    const viewModel = useCustomerViewModel();

    const { control, errors, handleSubmit } = viewModel.getNewCustomerForm();

    return (
        <ScrollView
            style={[formStyle.container, { backgroundColor: theme.colors.background }]}
            contentContainerStyle={formStyle.elements}
        >
            <ThemedInput
                control={control}
                name="name"
                error={errors.name}
                placeholder="Nom"
            />
            <ThemedInput
                control={control}
                name="code"
                error={errors.code}
                placeholder="Code"
            />
            <ThemedInput
                control={control}
                name="email"
                error={errors.email}
                placeholder="E-mail"
            />
            <ThemedInput
                control={control}
                name="phone"
                error={errors.phone}
                placeholder="Téléphone"
            />
            <ThemedInput
                control={control}
                name="address"
                error={errors.address}
                placeholder="Adresse"
            />
            <ThemedInput
                control={control}
                name="discount"
                error={errors.options?.discount}
                placeholder="Remise"
            />
            <ThemedCheckbox
                control={control}
                name="canValidateWithPhoto"
                error={errors.options?.canValidateWithPhoto}
                label="Valider avec photo"
            />
            <ThemedCheckbox
                control={control}
                name="canValidateWithSignature"
                error={errors.options?.canValidateWithSignature}
                label="Valider avec signature"
            />
            <ThemedCheckbox
                control={control}
                name="canValidateWithFile"
                error={errors.options?.canValidateWithFile}
                label="Valider avec fichier"
            />
            <Button
                mode="outlined"
                onPress={() => { }}
            >
                <Text>Créer le client</Text>
            </Button>
        </ScrollView>
    );
}