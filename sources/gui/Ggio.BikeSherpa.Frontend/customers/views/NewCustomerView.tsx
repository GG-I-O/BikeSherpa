import formStyle from "@/style/formStyle";
import { ScrollView } from "react-native";
import { Button, Text, useTheme } from "react-native-paper";
import ThemedInput from "@/components/themed/ThemedInput";
import ThemedCheckbox from "@/components/themed/ThemedCheckbox";
import { useNewCustomerForm } from "../hooks/useNewCustomerForm";
import { useState } from "react";

export default function NewCustomerView() {
    const theme = useTheme();

    const { control, errors, handleSubmit } = useNewCustomerForm();

    const [query, setQuery] = useState("");
    const [suggestions, setSuggestions] = useState<any[]>([]);
    const [formData, setFormData] = useState({
        streetInfo: "",
        postcode: "",
        city: "",
    });



    return (
        <ScrollView
            style={[formStyle.container, { backgroundColor: theme.colors.background }]}
            contentContainerStyle={formStyle.elements}
        >
            <ThemedInput
                control={control}
                name="name"
                error={errors.name}
                label="Nom"
                placeholder="Ma Petite Société"
                required
            />
            <ThemedInput
                control={control}
                name="code"
                error={errors.code}
                label="Code"
                placeholder="MPS"
                required
            />
            <ThemedInput
                control={control}
                name="email"
                error={errors.email}
                label="E-mail"
                placeholder="votre-nom@societe.fr"
                required
            />
            <ThemedInput
                control={control}
                name="phone"
                error={errors.phone}
                label="Téléphone"
                placeholder="06 10 11 12 13"
                required
            />
            <ThemedInput
                control={control}
                name="address"
                error={errors.address?.name}
                label="Adresse"
                placeholder="10 rue de la République 38100 Grenoble"
                required
            />
            <ThemedInput
                control={control}
                name="complement"
                error={errors.address?.complement}
                label="Complément d’adresse"
                placeholder="Bâtiment B"
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