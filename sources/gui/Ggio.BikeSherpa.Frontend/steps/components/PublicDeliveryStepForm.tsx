import {Button, Divider, IconButton, SegmentedButtons, Text, useTheme} from "react-native-paper";
import React, {Fragment, useCallback} from "react";
import {Control, FieldError, FieldErrors, Path, useController, useFieldArray} from "react-hook-form";
import ThemedAddressInput from "@/components/themed/ThemedAddressInput";
import {View} from "react-native";
import {PublicDeliveryFormValues} from "@/deliveries/models/zod/publicDeliveryFormBaseSchema";
import AppStyle from "@/constants/AppStyle";
import {StepType} from "@/steps/models/StepType";
import ThemedInput from "@/components/themed/ThemedInput";
import {PublicDeliveryCustomerTypeEnum} from "@/deliveries/data/PublicDeliveryCustomerType";
import ThemedDropdownInput from "@/components/themed/ThemedDropdownInput";
import {DropdownOptions} from "@/models/DropdownOptions";
import InputGroup from "@/components/themed/InputGroup";
import dateFilterDropdown from "@/deliveries/data/dateFilterDropdown";
import ThemedSegmentedInput from "@/components/themed/ThemedSegmentedInput";

type Props = {
    control: Control<PublicDeliveryFormValues>,
    errors: FieldErrors,
    packingSizes: DropdownOptions[],
    totalDistance: number,
    customerType: PublicDeliveryCustomerTypeEnum,
    canAddStep?: boolean
}

export default function PublicDeliveryStepForm(props: Props) {
    const theme = useTheme();
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
            packingSize: props.packingSizes.length > 0 ? props.packingSizes[0].value : 'S',
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
    }, [append, props.packingSizes]);

    return (
        <View style={{gap: 16, width: "100%"}}>
            {fields.map((step, index) => (
                <Fragment key={step.id}>
                    <View style={{flexDirection: 'row', flexWrap: 'wrap', gap: 8, alignItems: 'center'}}>
                        <View style={{flex: 1, gap: 16}}>
                            <View style={{
                                flexDirection: "row",
                                alignItems: "center",
                                justifyContent: 'center',
                                minWidth: '50%'
                            }}>
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
                            {step.stepType === StepType.Drop && (
                                <View style={{flex: 1, alignItems: 'center'}}>
                                    <Text style={AppStyle.textStyle.h3}>Pois et Taille du colis</Text>
                                    <ThemedSegmentedInput
                                        name={`steps.${index}.packingSize`}
                                        control={props.control}
                                        label=""
                                        buttons={props.packingSizes}
                                        error={props.errors.steps ? (props.errors.steps as any)[index]?.packingSize as FieldError | undefined : undefined}
                                    />
                                </View>
                            )}
                        </View>
                        <Divider style={{height: 150, width: 1}}/>
                        <View style={{flex: 1, minWidth: 280}}>
                            <InputGroup>
                                <ThemedInput
                                    testID="StepAddressName"
                                    control={props.control}
                                    name={`steps.${index}.stepAddress.name`}
                                    label="Société"
                                    placeholder=""
                                    error={props.errors.steps ? (props.errors.steps as any)[index]?.stepAddress.name as FieldError | undefined : undefined}
                                    horizontal
                                    labelIconCode={'briefcase'}
                                    disabled
                                />
                                <ThemedInput
                                    testID="StepAddressContact"
                                    control={props.control}
                                    name={`steps.${index}.contactName`}
                                    label="Contact"
                                    placeholder=""
                                    error={props.errors.steps ? (props.errors.steps as any)[index]?.contactName as FieldError | undefined : undefined}
                                    required
                                    horizontal
                                    labelIconCode={'account'}
                                />
                                <ThemedInput
                                    testID="StepAddressPhone"
                                    control={props.control}
                                    name={`steps.${index}.contactPhone`}
                                    label="Téléphone"
                                    placeholder=""
                                    error={props.errors.steps ? (props.errors.steps as any)[index]?.contactPhone as FieldError | undefined : undefined}
                                    horizontal
                                    labelIconCode={'phone'}
                                />
                                <ThemedInput
                                    testID="stepComment"
                                    control={props.control}
                                    name={`steps.${index}.comment`}
                                    label="Consignes"
                                    placeholder=""
                                    error={props.errors.steps ? (props.errors.steps as any)[index]?.comment as FieldError | undefined : undefined}
                                    horizontal
                                    labelIconCode={'file-document-outline'}
                                />
                            </InputGroup>
                        </View>
                    </View>
                    <Divider style={{height: 3}}/>
                </Fragment>
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