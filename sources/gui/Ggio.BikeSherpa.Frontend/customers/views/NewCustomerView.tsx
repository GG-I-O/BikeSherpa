import formStyle from "@/style/formStyle";
import { ScrollView } from "react-native";
import { Button, Text, useTheme } from "react-native-paper";
import ThemedInput from "@/components/themed/ThemedInput";
import { useNewCustomerForm } from "../hooks/useNewCustomerForm";
import ThemedAddressInput from "@/components/themed/ThemedAddressInput";

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
                name="phoneNumber"
                error={errors.phoneNumber}
                label="Téléphone"
                placeholder="06 10 11 12 13"
                required
            />
            <ThemedAddressInput
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
            <Button
                mode="outlined"
                onPress={() => handleSubmit()}
                style={formStyle.button}
            >
                <Text>Créer le client</Text>
            </Button>
        </ScrollView>
    );
}