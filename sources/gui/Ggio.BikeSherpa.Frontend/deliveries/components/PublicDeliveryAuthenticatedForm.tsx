import PublicDeliveryCustomer from "@/deliveries/models/PublicDeliveryCustomer";
import {Stack} from "expo-router";
import {Text} from "react-native-paper";

type Props = {
    customer: PublicDeliveryCustomer
}

export default function PublicDeliveryAuthenticatedForm(props: Props) {
    
    return (
        <>
            <Stack.Screen options={{title: `${props.customer.name} - Nouvelle course`}} />
            <Text>Authenticated form</Text>
        </>
    );
}