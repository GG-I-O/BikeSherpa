import {View} from "react-native";
import {Text} from "react-native-paper";

type Props = {
    price: number;
    priceWithTaxes: number;
}

export default function PublicDeliveryPrice(props: Props) { 
    return (
        <View>
            <Text>Prix HT: {props.price}€</Text>
            <Text>Prix TTC: {props.priceWithTaxes}€</Text>
            <Text>TVA: 20%</Text>
        </View>
    )
}