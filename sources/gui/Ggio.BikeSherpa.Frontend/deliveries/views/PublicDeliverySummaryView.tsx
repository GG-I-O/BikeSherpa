import {publicDeliveryStore$} from "@/deliveries/store/publicDeliveryStore";
import {View} from "react-native";
import {Divider, Text} from "react-native-paper";
import AppStyle from "@/constants/AppStyle";
import DateToolbox from "@/services/DateToolbox";

export default function PublicDeliverySummaryView() {
    
    const delivery = publicDeliveryStore$.delivery.get();
    const customer = publicDeliveryStore$.customer.get();
    const estimatedValue = publicDeliveryStore$.estimatedValue.get();
    
    if (!delivery || !customer || !estimatedValue)
        return (
            <View>
                <Text>Erreur lors du chargement du résumé de la commande</Text>
                <Text>Vérifiez vos emails pour obtenir la confirmation de votre commande</Text>
            </View>
        );
    
    return (
        <View style={{gap: 16, paddingInline: 32}}>
            <Text style={AppStyle.textStyle.h1}>{`Commande pour ${customer.name} `}</Text>
            <Text>Vous allez recevoir un mail de confirmation</Text>
            {delivery.steps.map((step, index) => (
                <View key={index} style={{gap: 8}}>
                    <Text style={AppStyle.textStyle.h2}>{step.stepType === 0 ? 'Adresse de ramasse' : 'Adresse de livraison'}</Text>
                    <Text style={{marginBottom: 8}}>{`${step.stepAddress.streetInfo} ${step.stepAddress.postcode} ${step.stepAddress.city}`}</Text>
                </View>
            ))}
            <Divider />
            <Text style={AppStyle.textStyle.h2}>Détails de la livraison</Text>
            <Text>Date de la livraison: {DateToolbox.getFormattedDateFromISO(delivery.startDate)}</Text>
            <Text>Heure de ramasse: {DateToolbox.getFormattedTimeFromISO(delivery.startDate)}</Text>
            <Divider />
            <Text>Distance total: {estimatedValue.distance} km</Text>
            <Text>Prix HT: {estimatedValue.priceWithoutTaxes} km</Text>
            <Text style={AppStyle.textStyle.h3}>Prix TTC: {estimatedValue.priceWithTaxes} €</Text>
        </View>
    );
}