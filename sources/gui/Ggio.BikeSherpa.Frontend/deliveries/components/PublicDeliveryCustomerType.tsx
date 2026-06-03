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
        <View style={{flexDirection: 'row', alignItems: 'center'}}>
            <Text style={AppStyle.textStyle.h3}>Je suis </Text>
            <SegmentedButtons
                value={props.customerType.toString()}
                onValueChange={(value) => props.setCustomerType(parseInt(value))}
                buttons={PublicDeliveryCustomerTypeOptions.map(b => ({
                    ...b,
                    style: {width: 150}
                }))}
            />
        </View>
    );
}