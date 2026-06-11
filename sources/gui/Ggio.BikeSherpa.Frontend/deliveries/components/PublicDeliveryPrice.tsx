import {View} from "react-native";
import {Text} from "react-native-paper";
import {Control} from "react-hook-form";
import {PublicDeliveryFormValues} from "@/deliveries/models/zod/publicDeliveryFormBaseSchema";
import ThemedCheckboxInput from "@/components/themed/ThemedCheckboxInput";
import usePublicDeliveryPriceViewModel from "@/deliveries/viewModel/usePublicDeliveryPriceViewModel";

type Props = {
    control: Control<PublicDeliveryFormValues>;
    price: number;
    priceWithTaxes: number;
}

export default function PublicDeliveryPrice(props: Props) {
    const viewModel = usePublicDeliveryPriceViewModel();
    
    return (
        <View style={{gap: 16}}>
            <ThemedCheckboxInput 
                name="needEstimate" 
                control={props.control} 
                label="Demander un devis"
            />
            <Text>Prix HT: {props.price.toFixed(2)}€</Text>
            <Text>Prix TTC: {props.priceWithTaxes.toFixed(2)}€</Text>
            <Text>TVA: {viewModel.vatRate}%</Text>
        </View>
    );
}