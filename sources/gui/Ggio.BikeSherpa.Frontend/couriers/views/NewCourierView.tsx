import formStyle from "@/style/formStyle";
import { useState } from "react";
import { ScrollView } from "react-native";
import { Button, Text, TextInput, useTheme } from "react-native-paper";

export default function NewCourierView() {
    const theme = useTheme();

    const [surname, setSurname] = useState<string>();
    const [firstname, setFirstname] = useState<string>();
    const [code, setCode] = useState<string>();

    return (
        <ScrollView
            style={[formStyle.container, { backgroundColor: theme.colors.background }]}
            contentContainerStyle={formStyle.elements}
        >
            <TextInput
                style={formStyle.input}
                label="Nom"
                value={surname}
                onChangeText={text => setSurname(text)}
            />
            <TextInput
                style={formStyle.input}
                label="Prénom"
                value={firstname}
                onChangeText={text => setFirstname(text)}
            />
            <TextInput
                style={formStyle.input}
                label="Code"
                value={code}
                onChangeText={text => setCode(text)}
            />
            <Button
                mode="outlined"
                onPress={() => { }}
            >
                <Text>Créer le livreur</Text>
            </Button>
        </ScrollView>
    );
}