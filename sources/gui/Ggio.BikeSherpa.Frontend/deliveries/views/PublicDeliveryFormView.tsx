import PublicDeliveryAnonymousForm from "@/deliveries/components/PublicDeliveryAnonymousForm";
import PublicDeliveryAuthenticatedForm from "@/deliveries/components/PublicDeliveryAuthenticatedForm";
import {publicDeliveryStore$} from "@/deliveries/store/publicDeliveryStore";
import {Stack} from "expo-router";

export default function PublicDeliveryFormView() {
    const customer = publicDeliveryStore$.customer.get();
    const isAnonymous = publicDeliveryStore$.isAnonymous.get();

    if (customer && !isAnonymous)
        return (
            <>
                <Stack.Screen options={{title: `${customer.name} - Nouvelle livraison`}}/>
                <PublicDeliveryAuthenticatedForm customer={customer}/>
            </>
        );

    return <PublicDeliveryAnonymousForm/>
}
