import {useLocalSearchParams} from "expo-router";
import {useEffect} from "react";
import {Linking, View} from "react-native";
import {Button, Text, useTheme} from "react-native-paper";
import useDeliveryDetailViewModel from "@/deliveries/viewModel/useDeliveryDetailViewModel";
import ReportDetail from "@/reports/components/ReportDetail";

export default function DeliveryDetailView() {
    const theme = useTheme();

    const {deliveryId} = useLocalSearchParams<{ deliveryId: string }>();
    const viewModel = useDeliveryDetailViewModel();

    useEffect(() => {
        if (!deliveryId) return;

        if (!viewModel.deliveryReport)
            viewModel.loadDeliveryReport(deliveryId).then();
    }, [deliveryId, viewModel]); // eslint-disable-line react-hooks/exhaustive-deps

    if (!viewModel.deliveryReport)
        return (
            <View style={{width: '100%', height: '100%', alignItems: 'center', justifyContent: 'center'}}>
                <Text>Course inexistante ou Chargement</Text>
            </View>
        );

    return (
        <View style={{backgroundColor: theme.colors.background, padding: 8, height: '100%'}}>
            <View style={{justifyContent: 'space-between', marginBottom: 24, flexWrap: 'wrap', gap: 8}}>
                <Button mode={"outlined"} onPress={() => viewModel.exportProofOfDelivery(deliveryId)}>
                    Envoyer une preuve de livraison
                </Button>
                {viewModel.proofOfDeliveryLink &&
                    <View style={{flexDirection: 'row', gap: 4, width: '100%'}}>
                        <Text>preuve de livraison exporté :</Text>
                        <Button
                            mode={"outlined"}
                            onPress={() => Linking.openURL(viewModel.proofOfDeliveryLink!)}
                        >
                            {viewModel.proofOfDeliveryLink}
                        </Button>
                    </View>
                }
            </View>
            <ReportDetail report={viewModel.deliveryReport}/>
        </View>
    );
}