import {Control, FieldError, FieldErrors} from "react-hook-form";
import {StyleSheet, View} from "react-native";
import {Text} from "react-native-paper";
import ThemedInput from "@/components/themed/ThemedInput";
import ThemedAddressInput from "@/components/themed/ThemedAddressInput";
import React from "react";
import {
    PublicDeliveryCustomerTypeEnum
} from "@/deliveries/data/PublicDeliveryCustomerType";
import AppStyle from "@/constants/AppStyle";
import {PublicDeliveryCustomerFormValues} from "@/deliveries/models/zod/publicDeliveryCustomerFormBaseSchema";

type Props = {
    control: Control<PublicDeliveryCustomerFormValues>;
    errors: FieldErrors;
    customerType: PublicDeliveryCustomerTypeEnum;
    knownCustomer: boolean;
}

export default function PublicDeliveryCustomerForm(props: Props) {

    const FormCell = ({grow = 1, minWidth = 200, children}: {
        grow?: number;
        minWidth?: number;
        children: React.ReactNode;
    }) => (
        <View style={{flexGrow: grow, flexBasis: minWidth, maxWidth: '100%'}}>
            {children}
        </View>
    );

    const customerErrors = (props.errors.customer as any);

    return (
        <View style={{gap: 16}}>
            <View style={styles.row}>
                <FormCell grow={3} minWidth={300}>
                    <ThemedAddressInput
                        control={props.control}
                        name="customer.address"
                        error={customerErrors?.address?.fullAddress as FieldError | undefined}
                        label="Adresse"
                        required
                        width={'100%'}
                    />
                </FormCell>
            </View>
            <View style={styles.row}>
                <FormCell grow={3} minWidth={260}>
                    <ThemedInput
                        testID="customerFormEmailInput"
                        control={props.control}
                        name="customer.email"
                        error={customerErrors?.email as FieldError | undefined}
                        label="E-mail"
                        placeholder="votre-nom@societe.fr"
                        required
                        disabled={props.knownCustomer}
                        width={'100%'}
                    />
                </FormCell>
                <FormCell grow={1} minWidth={120}>
                    <ThemedInput
                        testID="customerFormCodeInput"
                        control={props.control}
                        name="customer.code"
                        error={customerErrors?.code as FieldError | undefined}
                        label="ID Client"
                        placeholder="XXX"
                        disabled
                        width={'100%'}
                    />
                </FormCell>
            </View>

            <View style={styles.row}>
                <FormCell grow={1} minWidth={220}>
                    <ThemedInput
                        testID="customerFormNameInput"
                        control={props.control}
                        name="customer.name"
                        error={customerErrors?.name as FieldError | undefined}
                        label="Entreprise"
                        placeholder=""
                        required
                        width={'100%'}
                    />
                </FormCell>
                <FormCell grow={1} minWidth={220}>
                    <ThemedInput
                        testID="customerFormContactNameInput"
                        control={props.control}
                        name="customer.address.name"
                        error={customerErrors?.address?.name as FieldError | undefined}
                        label="Nom et Prénom"
                        placeholder=""
                        required
                        width={'100%'}
                    />
                </FormCell>
                <FormCell grow={1} minWidth={160}>
                    <ThemedInput
                        testID="customerFormPhoneInput"
                        control={props.control}
                        name="customer.phoneNumber"
                        error={customerErrors?.phoneNumber as FieldError | undefined}
                        label="Téléphone"
                        placeholder="06 10 11 12 13"
                        required
                        width={'100%'}
                    />
                </FormCell>
            </View>
        </View>
    );
}

const styles = StyleSheet.create({
    row: {flexDirection: 'row', flexWrap: 'wrap', gap: 8},
});