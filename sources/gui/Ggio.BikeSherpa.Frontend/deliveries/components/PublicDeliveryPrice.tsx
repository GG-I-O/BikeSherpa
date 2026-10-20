import {View} from "react-native";
import {Text, useTheme} from "react-native-paper";
import {Control} from "react-hook-form";
import {PublicDeliveryFormValues} from "@/deliveries/models/zod/publicDeliveryFormBaseSchema";
import usePublicDeliveryPriceViewModel from "@/deliveries/viewModel/usePublicDeliveryPriceViewModel";
import AppStyle from "@/constants/AppStyle";

type Props = {
    control: Control<PublicDeliveryFormValues>;
    price: number;
    priceWithTaxes: number;
}

export default function PublicDeliveryPrice(props: Props) {
    const theme = useTheme();

    const viewModel = usePublicDeliveryPriceViewModel();

    return (
        <View style={{gap: 16, flexDirection: 'row', flexWrap: 'wrap', justifyContent: 'space-evenly'}}>
            <View style={{flexDirection: 'row', gap: 8}}>
                <View style={{
                    alignSelf: 'flex-start',
                }}>
                    <Text style={AppStyle.textStyle.h3}>
                        Montant HT:
                    </Text>
                </View>
                <Text style={AppStyle.textStyle.h3}>
                    {props.price.toFixed(2)}€
                </Text>
            </View>

            <View style={{alignItems: 'flex-end'}}>
                <View style={{flexDirection: 'row', gap: 8}}>
                    <View style={{
                        alignSelf: 'flex-start',
                        borderBottomWidth: 1,
                        borderBottomColor: theme.colors.onBackground,
                    }}>
                        <Text style={[AppStyle.textStyle.h3, {lineHeight: undefined}]}>
                            Total TTC:
                        </Text>
                    </View>
                    <Text
                        style={[AppStyle.textStyle.h2, {
                            borderStyle: 'solid',
                            borderColor: theme.colors.onBackground,
                            borderWidth: 1,
                            paddingInline: 16
                        }]}>
                        {props.priceWithTaxes.toFixed(2)}€
                    </Text>
                </View>
                <Text>TVA: {viewModel.vatRate}%</Text>
            </View>
        </View>
    );
}