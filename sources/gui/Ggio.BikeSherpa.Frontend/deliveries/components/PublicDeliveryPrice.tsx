import {View} from "react-native";
import {Text} from "react-native-paper";
import {Control} from "react-hook-form";
import {PublicDeliveryFormValues} from "@/deliveries/models/zod/publicDeliveryFormBaseSchema";
import ThemedCheckboxInput from "@/components/themed/ThemedCheckboxInput";

type Props = {
    control: Control<PublicDeliveryFormValues>;
    price: number;
    priceWithTaxes: number;
}

export default function PublicDeliveryPrice(props: Props) { 
    
    const round = (num: number) => (Math.round(num * 100) / 100).toFixed(2);
    
    return (
        <View style={{gap: 16}}>
            <ThemedCheckboxInput 
                name="needEstimate" 
                control={props.control} 
                label="Demander un devis"
            />
            <Text>Prix HT: {round(props.price)}€</Text>
            <Text>Prix TTC: {round(props.priceWithTaxes)}€</Text>
            <Text>TVA: 20%</Text>
        </View>
    )
}