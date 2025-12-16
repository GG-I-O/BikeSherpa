import { useLocalSearchParams } from "expo-router";
import { useEffect, useState } from "react";
import { ScrollView, View } from "react-native";
import { Button, Text, TextInput, useTheme } from "react-native-paper";
import StepInputDataTable from "@/steps/components/inputs/StepInputDataTable";
import { Delivery } from "../models/Delivery";
import { Step } from "@/steps/models/Step";
import formStyle from "@/style/formStyle";
import useDeliveryViewModel from "../viewModel/DeliveryViewModel";

export default function DeliveryCopyView() {
    const { deliveryId } = useLocalSearchParams<{ deliveryId: string }>();

    const viewModel = useDeliveryViewModel();

    const [delivery, setDelivery] = useState<Delivery | undefined>();
    const [code, setCode] = useState<string>("");
    const [customer, setCustomer] = useState<string>("");
    const [steps, setSteps] = useState<Step[]>([]);

    useEffect(() => {
        const auxDelivery = viewModel.getDelivery(deliveryId);
        if (!auxDelivery)
            return;
        setDelivery(auxDelivery);
        setCode(auxDelivery.code);
        setCustomer(auxDelivery.customer.name);
        setSteps(auxDelivery.steps ?? []);

    }, [viewModel, deliveryId]);

    const theme = useTheme();

    if (!delivery)
        return (
            <View style={{ alignItems: 'center', justifyContent: 'center' }}>
                <Text>Course inexistante</Text>
            </View>
        )

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