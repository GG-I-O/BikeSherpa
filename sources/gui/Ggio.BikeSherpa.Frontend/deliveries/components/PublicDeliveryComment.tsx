import {Control, FieldError, FieldErrors} from "react-hook-form";
import {View} from "react-native";
import {Text} from "react-native-paper";
import AppStyle from "@/constants/AppStyle";
import React from "react";
import ThemedInput from "@/components/themed/ThemedInput";
import {PublicDeliveryCustomerFormValues} from "@/deliveries/models/zod/publicDeliveryCustomerFormBaseSchema";

type Props = {
    control: Control<PublicDeliveryCustomerFormValues>;
    errors: FieldErrors;
}

export default function PublicDeliveryComment(props: Props) {

    return (
        <View style={{gap: 8}}>
            <Text style={[AppStyle.textStyle.h2, {textAlign: 'center'}]}>
                Commentaires éventuels
            </Text>
            <ThemedInput
                name="deliveryComment"
                control={props.control}
                label=""
                placeholder="ref client, demandes complémentaires, etc..."
                error={props.errors.deliveryComment as FieldError | undefined}
                width={"100%"}
            />
        </View>
    );
}