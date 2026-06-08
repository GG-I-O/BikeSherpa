import {Button, IconButton, Text} from "react-native-paper";
import React, {useCallback} from "react";
import {Control, FieldError, FieldErrors, useFieldArray} from "react-hook-form";
import ThemedAddressInput from "@/components/themed/ThemedAddressInput";
import {View} from "react-native";
import {PublicDeliveryFormValues} from "@/deliveries/models/zod/publicDeliveryFormBaseSchema";
import AppStyle from "@/constants/AppStyle";
import {StepType} from "@/steps/models/StepType";
import ThemedInput from "@/components/themed/ThemedInput";
import {PublicDeliveryCustomerTypeEnum} from "@/deliveries/data/PublicDeliveryCustomerType";

type Props = {
    control: Control<PublicDeliveryFormValues>,
    errors: FieldErrors,
    totalDistance: number,
    customerType: PublicDeliveryCustomerTypeEnum,
    canAddStep?: boolean
}

export default function PublicDeliveryStepForm(props: Props) {

    const {fields, append, remove} = useFieldArray({
        name: "steps",
        control: props.control
    });

    const addStep = useCallback((stepType: number = 1) => {
        append({
            stepType: stepType,
            comment: '',
            courierComment: '',
            notBilled: false,
            contactName: '',
            contactPhone: '',
            stepAddress: {
                name: '',
                fullAddress: '',
                streetInfo: '',
                complement: '',
                postcode: '',
                city: '',
                coordinates: {longitude: 0, latitude: 0}
            }
        });
    }, [append]);

    return (
        <View style={{gap: 16, width: "100%"}}>
            <Text style={AppStyle.textStyle.h2}>Adresses de livraison</Text>
            {fields.map((step, index) => (
                <View key={index} style={{marginBottom: 16}}>
                    <View style={{flexDirection: "row", alignItems: "flex-end"}}>
                        <ThemedAddressInput
                            name={`steps.${index}.stepAddress`}
                            control={props.control}
                            label={step.stepType === StepType.PickUp ? "Adresse/lieu d'enlèvement" : `Adresse/lieu de destination ${index}`}
                            error={props.errors.steps ? (props.errors.steps as any)[index]?.stepAddress.fullAddress as FieldError | undefined : undefined}
                            labelAsTitle
                            required
                        />
                        {index >= 2 &&
                            <IconButton
                                style={{margin: 0}}
                                contentStyle={{justifyContent: "flex-end"}}
                                icon="trash-can-outline"
                                onPress={() => remove(index)}/>
                        }
                    </View>
                    {
                        (props.customerType === PublicDeliveryCustomerTypeEnum.None ||
                            (props.customerType === PublicDeliveryCustomerTypeEnum.Receiver && index !== 1) ||
                            (props.customerType === PublicDeliveryCustomerTypeEnum.Sender && index !== 0))
                        && (
                            <View style={{flexDirection: "row", gap: 8, width: "30%"}}>

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
                            </View>
                        )}
                    <View>
                        <ThemedInput
                            testID="stepComment"
                            control={props.control}
                            name={`steps.${index}.comment`}
                            label="Infos de livraison"
                            placeholder=""
                            error={props.errors.steps ? (props.errors.steps as any)[index]?.comment as FieldError | undefined : undefined}
                        />
                    </View>
                </View>
            ))}
            {props.canAddStep &&
                <Button
                    onPress={() => addStep()}
                    mode="outlined"
                >
                    <Text>Ajouter une étape</Text>
                </Button>
            }
            <Text style={AppStyle.textStyle.h3}>{`Kilométrage estimatif : ${props.totalDistance} km`}</Text>
        </View>
    );
}