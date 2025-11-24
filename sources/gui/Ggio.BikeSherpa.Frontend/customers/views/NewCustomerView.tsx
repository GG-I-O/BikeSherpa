import formStyle from "@/style/formStyle";
import { useState } from "react";
import { ScrollView } from "react-native";
import { Button, Text, TextInput, useTheme } from "react-native-paper";

export default function NewCustomerView() {
    const theme = useTheme();

    const [name, setName] = useState<string>();
    const [code, setCode] = useState<string>();
    const [address, setAddress] = useState<string>();
    const [siret, setSiret] = useState<string>();
    const [comment, setComment] = useState<string>();


    return (
        <ScrollView
            style={[formStyle.container, { backgroundColor: theme.colors.background }]}
            contentContainerStyle={formStyle.elements}
        >
            <TextInput
                style={formStyle.input}
                label="Nom*"
                value={name}
                onChangeText={text => setName(text)}
            />
            <TextInput
                style={formStyle.input}
                label="Code*"
                value={code}
                onChangeText={text => setCode(text)}
            />
            <TextInput
                style={formStyle.input}
                label="Adresse*"
                value={address}
                onChangeText={text => setAddress(text)}
            />
            <TextInput
                style={formStyle.input}
                label="SIRET"
                value={siret}
                onChangeText={text => setSiret(text)}
            />
            <TextInput
                style={formStyle.input}
                label="Commentaire"
                value={comment}
                onChangeText={text => setComment(text)}
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