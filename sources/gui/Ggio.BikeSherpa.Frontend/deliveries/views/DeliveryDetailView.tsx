import { useLocalSearchParams } from "expo-router";
import useDeliveryViewModel from "../viewModel/DeliveryViewModel";
import { useEffect, useState } from "react";
import { Delivery } from "../models/Delivery";
import { Dimensions, View } from "react-native";
import { Button, Divider, Text, useTheme } from "react-native-paper";
import AppStyle from "@/constants/AppStyle";
import { navigate } from "expo-router/build/global-state/routing";
import ThemedCardList from "@/components/themed/ThemedCardList";
import { Step } from "@/steps/models/Step";
import StepCardAssign from "@/steps/components/StepCardAssign";
import StepCard from "@/steps/components/StepCard";
import StepDataTable from "@/steps/components/StepDataTable";

type Props = {
    canEdit?: boolean
}

export default function DeliveryDetailView({ canEdit = false }: Props) {
    const theme = useTheme();

    const { deliveryId } = useLocalSearchParams<{ deliveryId: string }>();
    const viewModel = useDeliveryViewModel();

    const [delivery, setDelivery] = useState<Delivery | undefined>();

    useEffect(() => {
        setDelivery(viewModel.getDelivery(deliveryId));
    }, [deliveryId, viewModel]);

    const screenWidth = Dimensions.get('window').width;

    if (!delivery)
        return (
            <View style={{ width: '100%', height: '100%', alignItems: 'center', justifyContent: 'center' }}>
                <Text>Course inexistante</Text>
            </View>
        );

    return (
        <View style={{ backgroundColor: theme.colors.background, padding: 8, height: '100%' }}>
            <View style={{ justifyContent: 'space-between', marginBottom: 24, flexWrap: 'wrap', gap: 8 }}>
                {canEdit ? (
                    <View style={{ flexDirection: 'row', justifyContent: 'flex-end', gap: 8 }}>
                        <Button
                            mode="outlined"
                            onPress={() => navigate({
                                pathname: '/(tabs)/(deliveries)/edit',
                                params: { deliveryId: delivery.id }
                            })}
                        >
                            <Text>Modifier</Text>
                        </Button>
                        <Button
                            mode="outlined"
                            onPress={() => navigate({
                                pathname: '/(tabs)/(deliveries)/copy',
                                params: { deliveryId: delivery.id }
                            })}
                        >
                            <Text>Copier</Text>
                        </Button>
                        <Button mode="outlined">
                            <Text>Supprimer</Text>
                        </Button>
                        <Divider />
                    </View>
                ) : (
                    <></>
                )}
                <Text style={[AppStyle.textStyle.h2, { marginBottom: 8 }]}>Course :</Text>
                <View style={{ flexDirection: 'row', justifyContent: 'space-between', width: '100%' }}>
                    <Text style={AppStyle.textStyle.h3}>{delivery.code}</Text>
                    <Text style={AppStyle.textStyle.h3}>{delivery.customer.name}</Text>
                </View>
            </View>
            <Text style={[AppStyle.textStyle.h2, { marginBottom: 8 }]}>Étapes :</Text>
            {
                screenWidth <= 992 ? (
                    <ThemedCardList
                        data={delivery.steps ?? []}
                        card={({ item }) => {
                            const step = item as Step;
                            if (canEdit)
                                return (
                                    <StepCardAssign
                                        step={step}
                                        onAssign={(step: Step, courier: string) => {
                                            viewModel.assignSteps(courier, [step]);
                                        }}
                                        onTimePicker={(step: Step, time: Date) => {
                                            step.estimatedDate = time;
                                        }}
                                    />
                                );
                            return (
                                <StepCard
                                    step={step}
                                />
                            );
                        }}
                    />
                ) : (
                    <StepDataTable
                        steps={delivery.steps ?? []}
                        showHeader={true}
                    />
                )
            }
        </View >
    );
}