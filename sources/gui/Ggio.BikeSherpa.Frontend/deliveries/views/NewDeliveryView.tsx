import StepInputDataTable from "@/steps/components/inputs/StepInputDataTable";
import { Step } from "@/steps/models/Step";
import formStyle from "@/style/formStyle";
import { useState } from "react";
import { ScrollView } from "react-native";
import { Button, Text, TextInput, useTheme } from "react-native-paper";

export default function NewDeliveryView() {
    const theme = useTheme();

    const [code, setCode] = useState<string>("");
    const [customer, setCustomer] = useState<string>("");

    const [steps, setSteps] = useState<Step[]>([]);

    return (
        <ScrollView
            style={[formStyle.container, { backgroundColor: theme.colors.background }]}
            contentContainerStyle={formStyle.elements}
        >
            <TextInput
                style={formStyle.input}
                label="Code"
                value={code}
                onChangeText={text => setCode(text)}
            />
            <TextInput
                style={formStyle.input}
                label="Client"
                value={customer}
                onChangeText={text => setCustomer(text)}
            />
            <Button
                mode="outlined"
                onPress={() => {
                    setSteps(steps.concat([]));
                }}
            >
                <Text>Ajouter une étape</Text>
            </Button>

            <StepInputDataTable
                steps={steps}
                deleteRow={(step) => setSteps(steps.filter((s) => s.id !== step.id))}
            />
            <Button
                mode="outlined"
                onPress={() => { }}
            >
                <Text>Créer la course</Text>
            </Button>
        </ScrollView>
    );
}