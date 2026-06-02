import {Control, FieldError, FieldErrors} from "react-hook-form";
import {View} from "react-native";
import {Text} from "react-native-paper";
import ThemedInput from "@/components/themed/ThemedInput";
import ThemedAddressInput from "@/components/themed/ThemedAddressInput";
import React from "react";
import {
    PublicDeliveryCustomerTypeEnum
} from "@/deliveries/data/PublicDeliveryCustomerType";
import {PublicDeliveryFormValues} from "@/deliveries/models/zod/publicDeliveryFormBaseSchema";

type Props = {
    control: Control<PublicDeliveryFormValues>;
    errors: FieldErrors;
    customerType: PublicDeliveryCustomerTypeEnum;
}

export default function PublicDeliveryCustomerForm(props: Props) {

    return (
        <View>
            <Text>Informations du client / facturation</Text>
            <ThemedInput
                testID="customerFormNameInput"
                control={props.control}
                name="name"
                error={(props.errors.customer as any)?.name as FieldError | undefined}
                label="Nom"
                placeholder="Ma Petite Société"
                required
            />
            <ThemedInput
                testID="customerFormEmailInput"
                control={props.control}
                name="email"
                error={(props.errors.customer as any)?.email as FieldError | undefined}
                label="E-mail"
                placeholder="votre-nom@societe.fr"
                required
            />
            <ThemedInput
                testID="customerFormPhoneInput"
                control={props.control}
                name="phoneNumber"
                error={(props.errors.customer as any)?.phoneNumber as FieldError | undefined}
                label="Téléphone"
                placeholder="06 10 11 12 13"
                required
            />
            {props.customerType === PublicDeliveryCustomerTypeEnum.None && (
                <>
                    <ThemedAddressInput
                        control={props.control}
                        name="customer.address"
                        error={(props.errors.customer as any)?.address.name as FieldError | undefined}
                        label="Adresse"
                        required
                    />
                    <ThemedInput
                        testID="customerFormComplementInput"
                        control={props.control}
                        name="customer.address.complement"
                        error={(props.errors.customer as any)?.address.complement as FieldError | undefined}
                        label="Complément d’adresse"
                        placeholder="Bâtiment B"
                    />
                </>
            )}
        </View>
    );
}