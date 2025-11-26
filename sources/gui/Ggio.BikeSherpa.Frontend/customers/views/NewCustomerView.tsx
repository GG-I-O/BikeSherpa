import formStyle from "@/style/formStyle";
import { ScrollView } from "react-native";
import { Button, Text, useTheme } from "react-native-paper";
import ThemedInput from "@/components/themed/ThemedInput";
import ThemedCheckbox from "@/components/themed/ThemedCheckbox";
import { useNewCustomerForm } from "../hooks/useNewCustomerForm";

export default function NewCustomerView() {
    const theme = useTheme();

    const { control, errors, handleSubmit } = useNewCustomerForm();

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
                onPress={() => handleSubmit()}
            >
                <Text>Créer le client</Text>
            </Button>
        </ScrollView>
    );
}