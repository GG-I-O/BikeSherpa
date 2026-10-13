import {View} from "react-native";
import {Divider, Text, useTheme} from "react-native-paper";
import AppStyle from "@/constants/AppStyle";
import DateToolbox from "@/services/DateToolbox";
import usePublicDeliverySummaryViewModel from "@/deliveries/viewModel/usePublicDeliverySummaryViewModel";
import PublicDeliveryCustomerForm from "@/deliveries/components/PublicDeliveryCustomerForm";
import PublicDeliveryCustomerType from "@/deliveries/components/PublicDeliveryCustomerType";

export default function PublicDeliverySummaryView() {
    const theme = useTheme();

    const viewModel = usePublicDeliverySummaryViewModel();
    const {delivery, customer, estimatedValue} = viewModel;

    if (!delivery || !customer || !estimatedValue)
        return (
            <View style={{backgroundColor: theme.colors.background, alignItems: 'center', justifyContent: 'center'}}>
                <Text>Erreur lors du chargement de la commande</Text>
            </View>
        );

    return (
        <View style={{gap: 16, paddingInline: 32, backgroundColor: theme.colors.background}}>
            <Text style={AppStyle.textStyle.h1}>{`Récapitulatif de la commande pour ${customer.name} `}</Text>
            {delivery.steps.map((step, index) => (
                <View key={index} style={{gap: 8}}>
                    <Text
                        style={AppStyle.textStyle.h2}>
                        {step.stepType === 0 ? 'Adresse de ramasse' : 'Adresse de livraison'}
                    </Text>
                    <Text
                        style={{marginBottom: 8}}>
                        {`${step.stepAddress.streetInfo} ${step.stepAddress.postcode} ${step.stepAddress.city}`}
                    </Text>
                </View>
            ))}
            <Divider/>
            <Text style={AppStyle.textStyle.h2}>Détails de la livraison</Text>
            <Text>Date de la livraison: {DateToolbox.getFormattedDateFromISO(delivery.startDate)}</Text>
            <Text>Horaire de début de livraison: {DateToolbox.getFormattedTimeFromISO(delivery.startDate)}</Text>
            <Text>Durée créneau de livraison: {viewModel.urgencyLabel?.value ?? ''}</Text>
            <Divider/>
            <Text>Montant HT: {estimatedValue.price} km</Text>
            <Text style={AppStyle.textStyle.h3}>Total TTC: {estimatedValue.priceWithVat} €</Text>
            <Divider/>
            <PublicDeliveryCustomerType
                customerType={viewModel.customerType}
                setCustomerType={viewModel.setCustomerType}
            />
            <PublicDeliveryCustomerForm
                control={viewModel.control}
                errors={viewModel.errors}
                customerType={viewModel.customerType}
            />
        </View>
    );
}