import {useLocalSearchParams} from "expo-router";
import {useEffect, useState} from "react";
import {View} from "react-native";
import {Button, Divider, Text, useTheme} from "react-native-paper";
import AppStyle from "@/constants/AppStyle";
import {navigate} from "expo-router/build/global-state/routing";
import StepDataTable from "@/steps/components/StepDataTable";
import useDeliveryDetailViewModel from "@/deliveries/viewModel/useDeliveryDetailViewModel";
import {DeliveryToDisplay} from "@/deliveries/models/DeliveryToDisplay";

type Props = {
    canEdit?: boolean
}

export default function DeliveryDetailView({canEdit = false}: Props) {
    const theme = useTheme();

    const {deliveryId} = useLocalSearchParams<{ deliveryId: string }>();
    const viewModel = useDeliveryDetailViewModel();

    const [delivery, setDelivery] = useState<DeliveryToDisplay | undefined>();

    useEffect(() => {
        setDelivery(viewModel.getDelivery(deliveryId));
    }, [deliveryId, viewModel]);

    if (!delivery)
        return (
            <View style={{width: '100%', height: '100%', alignItems: 'center', justifyContent: 'center'}}>
                <Text>Course inexistante</Text>
            </View>
        );

    return (
        <View style={{backgroundColor: theme.colors.background, padding: 8, height: '100%'}}>
            <View style={{justifyContent: 'space-between', marginBottom: 24, flexWrap: 'wrap', gap: 8}}>
                {canEdit ? (
                    <View style={{flexDirection: 'row', justifyContent: 'flex-end', gap: 8}}>
                        <Button
                            mode="outlined"
                            onPress={() => navigate({
                                pathname: '/(tabs)/(deliveries)/edit',
                                params: {deliveryId: delivery.id}
                            })}
                        >
                            <Text>Modifier</Text>
                        </Button>
                        <Button
                            mode="outlined"
                            onPress={() => navigate({
                                pathname: '/(tabs)/(deliveries)/copy',
                                params: {deliveryId: delivery.id}
                            })}
                        >
                            <Text>Copier</Text>
                        </Button>
                        <Button mode="outlined">
                            <Text>Supprimer</Text>
                        </Button>
                        <Divider/>
                    </View>
                ) : (
                    <></>
                )}
                <Text style={[AppStyle.textStyle.h2, {marginBottom: 8}]}>Course :</Text>
                <View style={{flexDirection: 'row', justifyContent: 'space-between', width: '100%'}}>
                    <Text style={AppStyle.textStyle.h3}>{delivery.code}</Text>
                    <Text style={AppStyle.textStyle.h3}>{delivery.customerName}</Text>
                </View>
                <Text style={AppStyle.textStyle.h3}>{delivery.startDate}</Text>
                <Text style={AppStyle.textStyle.h3}>{delivery.urgency}</Text>
            </View>
            <Text style={[AppStyle.textStyle.h2, {marginBottom: 8}]}>Étapes :</Text>
            {
                <StepDataTable
                    steps={delivery.steps ?? []}
                    showHeader={true}
                />
            }
        </View>
    );
}