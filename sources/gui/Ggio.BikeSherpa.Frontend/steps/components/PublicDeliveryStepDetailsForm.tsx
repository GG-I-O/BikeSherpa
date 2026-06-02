import {SegmentedButtons, Text} from "react-native-paper";
import {Control, FieldError, FieldErrors, useFieldArray} from "react-hook-form";
import {View} from "react-native";
import {
    PublicDeliveryCustomerTypeEnum,
    PublicDeliveryCustomerTypeOptions
} from "@/deliveries/data/PublicDeliveryCustomerType";
import ThemedInput from "@/components/themed/ThemedInput";
import React from "react";
import {PublicDeliveryFormValues} from "@/deliveries/models/zod/publicDeliveryFormBaseSchema";

type Props = {
    control: Control<PublicDeliveryFormValues>;
    errors: FieldErrors
    customerType: PublicDeliveryCustomerTypeEnum;
    setCustomerType: (type: PublicDeliveryCustomerTypeEnum) => void;
}

export default function PublicDeliveryStepDetailsForm(props: Props) {

    const {fields} = useFieldArray({
        name: "steps",
        control: props.control
    });
    
    return (
        <View>
            <Text>Informations de livraison</Text>
            <SegmentedButtons
                value={props.customerType.toString()}
                onValueChange={(value) => props.setCustomerType(parseInt(value))}
                buttons={PublicDeliveryCustomerTypeOptions.map(b => ({
                    ...b,
                    style: {width: 100}
                }))}
            />
            {fields.map((step, index) => (
                <View key={`stepsDetails-${index}`}>
                    {
                        (props.customerType === PublicDeliveryCustomerTypeEnum.None ||
                            (props.customerType === PublicDeliveryCustomerTypeEnum.Receiver && index !== 0) ||
                            (props.customerType === PublicDeliveryCustomerTypeEnum.Sender && index !== 1)) && (
                            <>
                                {index === 0 && (
                                    <Text>Coordonées expéditeur</Text>
                                )}
                                {index === 1 && (
                                    <Text>Coordonées destinataire</Text>
                                )}
                                <Text>Rappel Adresse : {`${step.stepAddress.streetInfo} ${step.stepAddress.postcode} ${step.stepAddress.city}`}</Text>
                                <ThemedInput
                                    testID="StepAddressName"
                                    control={props.control}
                                    name={`steps.${index}.contactName`}
                                    label="Nom"
                                    placeholder=""
                                    error={(props.errors.steps as any)[index]?.contactName as FieldError | undefined}
                                    required
                                />
                                <ThemedInput
                                    testID="StepAddressPhone"
                                    control={props.control}
                                    name={`steps.${index}.contactPhone`}
                                    label="Téléphone"
                                    placeholder=""
                                    error={(props.errors.steps as any)[index]?.contactPhone as FieldError | undefined}
                                />
                            </>
                        )
                    }
                </View>
            ))}
        </View>
    );
}