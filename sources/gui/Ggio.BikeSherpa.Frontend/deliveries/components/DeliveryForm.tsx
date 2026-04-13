import ThemedDropdownInput from "@/components/themed/ThemedDropdownInput";
import ThemedInput from "@/components/themed/ThemedInput";
import formStyle from "@/style/formStyle";
import {Control, FieldError, FieldErrors, FieldValues, Path, useController} from "react-hook-form";
import {ScrollView} from "react-native";
import {Button, Text, useTheme} from "react-native-paper";
import React from "react";
import ThemedCheckboxInput from "@/components/themed/ThemedCheckboxInput";
import ThemedDateInput from "@/components/themed/ThemedDateInput";
import {DropdownOptions} from "@/models/DropdownOptions";

interface DeliveryFormProps<T extends FieldValues> {
    control: Control<T, any, T>;
    handleSubmit: (e?: React.BaseSyntheticEvent) => Promise<void>;
    errors: FieldErrors<T>;
    buttonName: string;
    urgencies: DropdownOptions[];
    pricingStrategies: DropdownOptions[];
    packingSizes: DropdownOptions[];
}

export default function DeliveryForm<T extends FieldValues>(props: DeliveryFormProps<T>) {
    const theme = useTheme();
    const {control, errors, handleSubmit, buttonName} = props;

    const {field} = useController({
        control,
        name: "pricingStrategy" as Path<T>,
    });

    return (
        <ScrollView
            style={[formStyle.container, {backgroundColor: theme.colors.background}]}
            contentContainerStyle={formStyle.elements}
        >
            <ThemedInput
                testID="deliveryFormCustomerInput"
                control={control}
                name="customerId"
                error={errors.customerId as FieldError | undefined}
                label="Code client"
                placeholder="CLI80"
                required
            />
            <ThemedDropdownInput
                testID="deliveryFormPricingStrategyInput"
                control={control}
                name="pricingStrategy"
                error={errors.pricingStrategy as FieldError | undefined}
                label="Calcul de prix"
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
                label="Prix total"
                placeholder="10"
                disabled={props.pricingStrategies.length === 0 || field.value !== parseInt(props.pricingStrategies[0].value)}
                required
            />
            <ThemedInput
                testID="deliveryFormDiscountInput"
                control={control}
                name="discount"
                error={errors.discount as FieldError | undefined}
                label="Remise"
                placeholder="0"
            />
            <ThemedInput
                testID="deliveryFormDetailsInput"
                control={control}
                name="details"
                error={errors.details as FieldError | undefined}
                label="Details"
                placeholder="Message lambda"
                isAnArray
            />
            <ThemedDropdownInput
                testID="deliveryFormPackingSizeInput"
                control={control}
                name="packingSize"
                error={errors.packingSize as FieldError | undefined}
                label="Taille"
                options={props.packingSizes}
                required
            />
            <ThemedCheckboxInput
                testID="deliveryFormInsulatedInput"
                control={control}
                name="insulatedBox"
                error={errors.insulatedBox as FieldError | undefined}
                label="Boite isolée"
                required
            />
            <ThemedDateInput
                testID="deliveryFormContractDateInput"
                control={control}
                name="contractDate"
                error={errors.contractDate as FieldError | undefined}
                label="Date du contrat"
                placeholder="30/01/2000"
                required
            />
            <ThemedDateInput
                testID="deliveryFormStartDateInput"
                control={control}
                name="startDate"
                error={errors.startDate as FieldError | undefined}
                label="Date de début"
                placeholder="30/01/2000"
                required
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