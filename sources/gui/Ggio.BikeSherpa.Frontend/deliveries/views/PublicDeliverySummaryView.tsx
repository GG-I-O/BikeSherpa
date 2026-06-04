import {publicDeliveryStore$} from "@/deliveries/store/publicDeliveryStore";
import {View} from "react-native";
import {Divider, Text} from "react-native-paper";

export default function PublicDeliverySummaryView() {
    
    const delivery = publicDeliveryStore$.delivery.get();
    const customer = publicDeliveryStore$.customer.get();
    const estimatedValue = publicDeliveryStore$.estimatedValue.get();
    
    console.log("Summary")
    console.log(delivery)
    console.log(customer)
    console.log(estimatedValue)
    
    if (!delivery || !customer || !estimatedValue)
        return (
            <View>
                <Text>Erreur lors du chargement du résumé de la commande</Text>
                <Text>Vérifiez vos emails pour obtenir la confirmation de votre commande</Text>
            </View>
        );
    
    return (
        <View>
            <Text>{`Commande pour ${customer.name} `}</Text>
            {delivery.steps.map((step, index) => (
                <View key={index} style={{gap: 8}}>
                    <Text>{step.stepType === 0 ? 'Adresse de ramasse' : 'Adresse de livraison'}</Text>
                    <Text style={{marginBottom: 8}}>{`${step.stepAddress.streetInfo} ${step.stepAddress.postcode} ${step.stepAddress.city}`}</Text>
                </View>
            ))}
            <Divider />
            <Text>Distance total: {estimatedValue.distance} km</Text>
            <Text>Prix HT: {estimatedValue.priceWithoutTaxes} km</Text>
            <Text>Prix TTC: {estimatedValue.priceWithTaxes} €</Text>
        </View>
    );
}