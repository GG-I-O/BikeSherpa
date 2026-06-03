import {Text} from "react-native-paper";
import {Control, FieldError, FieldErrors, useWatch} from "react-hook-form";
import {View} from "react-native";
import {
    PublicDeliveryCustomerTypeEnum,
} from "@/deliveries/data/PublicDeliveryCustomerType";
import ThemedInput from "@/components/themed/ThemedInput";
import React from "react";
import {PublicDeliveryFormValues} from "@/deliveries/models/zod/publicDeliveryFormBaseSchema";
import AppStyle from "@/constants/AppStyle";

type Props = {
    control: Control<PublicDeliveryFormValues>;
    errors: FieldErrors
    customerType: PublicDeliveryCustomerTypeEnum;
}

export default function PublicDeliveryStepDetailsForm(props: Props) {

    const steps = useWatch({
        name: "steps",
        control: props.control,
        defaultValue: []
    });

    if (!steps || steps.length === 0)
        return <></>

    return (
        <View>
            <Text style={AppStyle.textStyle.h2}>Informations de livraison</Text>
            {steps.map((step, index) => (
                <View key={`stepsDetails-${index}`} style={{gap: 8, marginTop: 16}}>
                    {
                        (props.customerType === PublicDeliveryCustomerTypeEnum.None ||
                            (props.customerType === PublicDeliveryCustomerTypeEnum.Receiver && index !== 1) ||
                            (props.customerType === PublicDeliveryCustomerTypeEnum.Sender && index !== 0)) && (
                            <>
                                {index === 0 && (
                                    <Text style={AppStyle.textStyle.h3}>Coordonées expéditeur</Text>
                                )}
                                {index === 1 && (
                                    <Text style={AppStyle.textStyle.h3}>Coordonées destinataire</Text>
                                )}
                                <Text>Rappel Adresse
                                    : {step.stepAddress ? `${step.stepAddress.streetInfo} ${step.stepAddress.postcode} ${step.stepAddress.city}` : ''}</Text>
                                <ThemedInput
                                    testID="StepAddressName"
                                    control={props.control}
                                    name={`steps.${index}.contactName`}
                                    label="Nom"
                                    placeholder=""
                                    error={props.errors.steps ? (props.errors.steps as any)[index]?.contactName as FieldError | undefined : undefined}
                                    required
                                />
                                <ThemedInput
                                    testID="StepAddressPhone"
                                    control={props.control}
                                    name={`steps.${index}.contactPhone`}
                                    label="Téléphone"
                                    placeholder=""
                                    error={props.errors.steps ? (props.errors.steps as any)[index]?.contactPhone as FieldError | undefined : undefined}
                                />
                            </>
                        )
                    }
                </View>
            ))}
        </View>
    );
}