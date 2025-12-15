import AppStyle from "@/constants/AppStyle";
import { useLocalSearchParams } from "expo-router";
import { navigate } from "expo-router/build/global-state/routing";
import { useEffect, useState } from "react";
import { View } from "react-native";
import { Button, Text, useTheme } from "react-native-paper";
import { Delivery } from "@/deliveries/models/Delivery";
import { Step } from "../models/Step";
import useDeliveryViewModel from "../../deliveries/viewModel/DeliveryViewModel";
import { StepType } from "../models/StepType";

type Props = {
    canEdit?: boolean,
    canValidate?: boolean
}

export default function StepDetailView({ canEdit = false }: Props) {
    const theme = useTheme();

    const { stepId } = useLocalSearchParams<{ stepId: string }>();

    const viewModel = useDeliveryViewModel();

    const [step, setStep] = useState<Step | undefined>()
    const [delivery, setDelivery] = useState<Delivery | undefined>();

    useEffect(() => {
        if (!step)
            setStep(viewModel.getStep(stepId));
        if (step)
            setDelivery(viewModel.getDeliveryByCode(step.id));
    }, [stepId, viewModel, step]);

    if (!step || !delivery)
        return (
            <View style={{ alignItems: 'center', justifyContent: 'center' }}>
                <Text>Étape inexistante</Text>
            </View>
        )

    return (
        <View style={{ backgroundColor: theme.colors.background, padding: 8, height: '100%' }}>
            <Text style={[AppStyle.textStyle.h2, { marginBottom: 8 }]}>Course :</Text>
            <View style={{ flexDirection: 'row', justifyContent: 'space-between', marginBottom: 24, flexWrap: 'wrap', gap: 8 }}>
                <Text style={AppStyle.textStyle.h3}>{delivery.code}</Text>
                <Text style={AppStyle.textStyle.h3}>{delivery.customer.name}</Text>
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
                    </View>
                ) : (
                    <></>
                )}
            </View>
            <Text style={[AppStyle.textStyle.h2, { marginBottom: 8 }]}>Étape :</Text>
            <View style={{ gap: 32 }}>
                <View style={{ flexDirection: 'row', justifyContent: 'space-evenly', flexWrap: 'wrap', width: '100%' }}>
                    <View style={{ justifyContent: 'center' }}>
                        <Text style={AppStyle.textStyle.h3}>Expéditeur</Text>
                        <Text>Nom Prénom</Text>
                        <Text>1 Rue de l’Expéditeur</Text>
                        <Text>38000 Grenoble</Text>
                    </View>
                    <View style={{ justifyContent: 'center' }}>
                        <Text style={AppStyle.textStyle.h3}>Prise en charge</Text>
                        <Text>Nom Prénom</Text>
                        <Text>1 Rue du Destinataire</Text>
                        <Text>38000 Grenoble</Text>
                    </View>
                </View>
                <View style={{ justifyContent: 'center' }}>
                    <View style={{ justifyContent: 'flex-start', gap: 4 }}>
                        <View style={{ flexDirection: 'row', justifyContent: 'space-between' }}>
                            <Text>{step.type === StepType.PickUp ? 'Heure mini' : 'Heure max'} : {step.getContractTime()}</Text>
                            <Text style={AppStyle.textStyle.h3}>ETA : {step.getEstimatedTime()}</Text>
                        </View>
                        <Text>Description : {step.comment}</Text>
                        <Text style={[AppStyle.textStyle.h3, { textAlign: 'center' }]}>Infos livreur :</Text>
                        <Text style={{ textAlign: 'center', fontStyle: 'italic' }}>{step.comment}</Text>
                    </View>
                </View>
            </View>
            <View style={{ flexDirection: 'row', justifyContent: 'space-evenly', gap: 8, marginTop: 32 }}>
                <Button
                    mode="outlined"
                    onPress={() => { }}
                >
                    <Text>Photographier</Text>
                </Button>
                <Button
                    mode="outlined"
                    onPress={() => { }}
                >
                    <Text>Signer</Text>
                </Button>
                <Button
                    mode="outlined"
                    onPress={() => { }}
                >
                    <Text>Valider</Text>
                </Button>
            </View>
        </View >
    );
}