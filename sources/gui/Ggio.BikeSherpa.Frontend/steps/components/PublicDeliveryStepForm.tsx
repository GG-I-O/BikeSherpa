import {Button, IconButton, Text} from "react-native-paper";
import {useCallback} from "react";
import {Control, useFieldArray} from "react-hook-form";
import ThemedAddressInput from "@/components/themed/ThemedAddressInput";
import {View} from "react-native";
import {PublicDeliveryFormValues} from "@/deliveries/models/zod/publicDeliveryFormBaseSchema";
import AppStyle from "@/constants/AppStyle";

type Props = {
    control: Control<PublicDeliveryFormValues>,
    totalDistance: number,
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
                coordinates: { longitude: 0, latitude: 0 }
            }
        });
    }, [append]);
    
    return (
        <View style={{gap: 16}}>
            <Text style={AppStyle.textStyle.h2}>Adresses de livraison</Text>
            <ThemedAddressInput
                name={`steps.0.stepAddress`}
                control={props.control}
                label="Adresse/lieu d'enlèvement"
                required
            />
            <ThemedAddressInput
                name={`steps.1.stepAddress`}
                control={props.control}
                label="Adresse/lieu de destination"
                required
            />
            { props.canAddStep && 
                <Button
                    onPress={() => addStep()}
                >
                    <Text>Ajouter une étape</Text>
                </Button>
            }
            {fields.slice(2).map((step, index) => (
                <View key={index} style={{flexDirection: "row", alignItems: "center"}}>
                    <ThemedAddressInput
                        name={`steps.${index+2}.stepAddress`}
                        control={props.control}
                        label=""
                    />
                    <IconButton style={{ margin: 0 }} icon="trash-can-outline" onPress={() => remove(index+2)} />
                </View>
            ))}
            <Text style={AppStyle.textStyle.h3}>{`Kilométrage estimatif : ${props.totalDistance} km`}</Text>
        </View>
    );
}