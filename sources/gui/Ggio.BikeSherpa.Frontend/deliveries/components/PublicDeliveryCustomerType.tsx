import {SegmentedButtons, Text} from "react-native-paper";
import {
    PublicDeliveryCustomerTypeEnum,
    PublicDeliveryCustomerTypeOptions
} from "@/deliveries/data/PublicDeliveryCustomerType";
import {View} from "react-native";
import React from "react";
import AppStyle from "@/constants/AppStyle";

type Props = {
    customerType: PublicDeliveryCustomerTypeEnum;
    setCustomerType: (customerType: PublicDeliveryCustomerTypeEnum) => void;
}

export default function PublicDeliveryCustomerType(props: Props) {
    return (
        <View style={{flexDirection: 'row', flexWrap: 'wrap', alignItems: 'center', columnGap: 8, rowGap: 8}}>
            <Text style={AppStyle.textStyle.h3}>Je suis </Text>
            <SegmentedButtons
                style={{flexGrow: 1, flexShrink: 1, flexBasis: 240, maxWidth: 450}}
                value={props.customerType.toString()}
                onValueChange={(value) => props.setCustomerType(parseInt(value))}
                buttons={PublicDeliveryCustomerTypeOptions}
            />
        </View>
    );
}