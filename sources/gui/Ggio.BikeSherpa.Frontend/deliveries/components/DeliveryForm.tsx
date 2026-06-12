import ThemedDropdownInput from "@/components/themed/ThemedDropdownInput";
import ThemedInput from "@/components/themed/ThemedInput";
import formStyle from "@/style/formStyle";
import {Control, FieldError, FieldErrors, FieldValues, Path, useController} from "react-hook-form";
import {ScrollView, View} from "react-native";
import {Button, Text, useTheme} from "react-native-paper";
import React from "react";
import ThemedCheckboxInput from "@/components/themed/ThemedCheckboxInput";
import ThemedDateInput from "@/components/themed/ThemedDateInput";
import {DropdownOptions} from "@/models/DropdownOptions";
import StepInputDataTable from "@/steps/components/inputs/StepInputDataTable";
import ThemedTimeInput from "@/components/themed/ThemedTimeInput";
import {ThemedRHFSuggestiveInput} from "@/components/themed/ThemedRHFSuggestiveInput";
import Customer from "@/customers/models/Customer";

interface DeliveryFormProps<T extends FieldValues> {
    update?: boolean;
    control: Control<T, any, T>;
    handleSubmit: (e?: React.BaseSyntheticEvent) => Promise<void>;
    errors: FieldErrors<T>;
    buttonName: string;
    getCustomerOptions: (query: string) => Promise<Customer[]>;
    getCustomer?: (id: string) => Customer;
    urgencies: DropdownOptions[];
    pricingStrategies: DropdownOptions[];
}

export default function DeliveryForm<T extends FieldValues>(props: DeliveryFormProps<T>) {
    const theme = useTheme();
    const {control, errors, handleSubmit, buttonName} = props;

    // Used to disable a field, so we read the value here
    const {field} = useController({
        control,
        name: "pricingStrategy" as Path<T>,
    });

    return (
        <ScrollView
            style={[formStyle.container, {backgroundColor: theme.colors.background}]}
            contentContainerStyle={formStyle.elements}
        >
            <ThemedRHFSuggestiveInput<T, Customer, string>
                name="customerId"
                control={control}
                label="Code client"
                required
                placeholder="CL1"
                error={errors.customerId as FieldError | undefined}
                fetchSuggestions={props.getCustomerOptions}
                minLength={1}
                getOptionLabel={(customer: Customer) => `${customer.code} - ${customer.name}`}
                getOptionValue={customer => customer.id}
                getLabelFromValue={
                    props.getCustomer ?
                        customerId => {
                            const customer = props.getCustomer!(customerId);
                            return `${customer.code} - ${customer.name}`
                        } : undefined
                }
            />
            <ThemedDropdownInput
                testID="deliveryFormPricingStrategyInput"
                control={control}
                name="pricingStrategy"
                error={errors.pricingStrategy as FieldError | undefined}
                label="Type de livraison"
                options={props.pricingStrategies}
                required
                isNumber
            />
            <ThemedDropdownInput
                testID="deliveryFormUrgencyInput"
                control={control}
                name="urgency"
                error={errors.urgency as FieldError | undefined}
                label="Urgence"
                options={props.urgencies}
                required
            />
            <ThemedInput
                testID="deliveryFormPriceInput"
                control={control}
                name="totalPrice"
                error={errors.totalPrice as FieldError | undefined}
                label="Prix total (en €)"
                placeholder="10"
                disabled={props.pricingStrategies.length === 0 || field.value !== parseInt(props.pricingStrategies[0].value)}
                required
                isNumeric
            />
            <ThemedInput
                testID="deliveryFormDiscountInput"
                control={control}
                name="discount"
                error={errors.discount as FieldError | undefined}
                label="Remise (en €)"
                placeholder="0"
                isNumeric
            />
            <ThemedInput
                testID="deliveryFormDiscountInput"
                control={control}
                name="extraCost"
                error={errors.discount as FieldError | undefined}
                label="Surcout (en €)"
                placeholder="0"
                isNumeric
            />
            <ThemedInput
                testID="deliveryFormDetailsInput"
                control={control}
                name="distance"
                error={errors.details as FieldError | undefined}
                label="Distance (en Km)"
                placeholder="Message lambda"
                isNumeric
            />
            <ThemedInput
                testID="deliveryFormDetailsInput"
                control={control}
                name="details"
                error={errors.details as FieldError | undefined}
                label="Infos de la course"
                placeholder="Message lambda"
                isAnArray
            />
            <ThemedCheckboxInput
                testID="deliveryFormInsulatedInput"
                control={control}
                name="insulatedBox"
                error={errors.insulatedBox as FieldError | undefined}
                label="Caisson isotherme"
                required
            />
            <View style={[formStyle.intputContainer, {flexDirection: 'row'}]}>
                <ThemedDateInput
                    style={{marginRight: 16}}
                    testID="deliveryFormContractDateInput"
                    control={control}
                    name="contractDate"
                    error={errors.contractDate as FieldError | undefined}
                    label="Date de la demande"
                    required
                />
                <ThemedTimeInput
                    control={control}
                    name="contractDate"
                    label="Heure de la demande"
                    required
                />
            </View>
            <View style={[formStyle.intputContainer, {flexDirection: 'row'}]}>
                <ThemedDateInput
                    style={{marginRight: 16}}
                    testID="deliveryFormStartDateInput"
                    control={control}
                    name="startDate"
                    error={errors.startDate as FieldError | undefined}
                    label="Date de livraison"
                    required
                />
                <ThemedTimeInput
                    control={control}
                    name="startDate"
                    label="Heure de livraison"
                    required
                />
            </View>
            <StepInputDataTable
                control={control}
                errors={errors}
                name="steps"
            />
            <Button
                testID="formButton"
                mode="outlined"
                onPress={() => handleSubmit()}
                style={formStyle.button}
            >
                <Text testID="buttonName">{buttonName}</Text>
            </Button>
        </ScrollView>
    );
}