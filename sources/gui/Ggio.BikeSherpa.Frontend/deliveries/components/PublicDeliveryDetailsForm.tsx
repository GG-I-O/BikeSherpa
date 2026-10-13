import {Text, useTheme} from "react-native-paper";
import {View} from "react-native";
import ThemedDateInput from "@/components/themed/ThemedDateInput";
import {Control, FieldError, FieldErrors, useController} from "react-hook-form";
import formStyle from "@/style/formStyle";
import React, {useMemo} from "react";
import {Dropdown} from "react-native-paper-dropdown";
import ThemedDropdownInput from "@/components/themed/ThemedDropdownInput";
import ThemedCheckboxInput from "@/components/themed/ThemedCheckboxInput";
import {PublicDeliveryFormValues} from "@/deliveries/models/zod/publicDeliveryFormBaseSchema";
import AppStyle from "@/constants/AppStyle";
import usePublicDeliveryDetailsFormViewModel from "@/deliveries/viewModel/usePublicDeliveryDetailsFormViewModel";
import CompactDropdown from "@/components/general/CompactDropdown";

type Props = {
    control: Control<PublicDeliveryFormValues>;
    errors: FieldErrors;
    setUrgency: (urgency: string) => void;
    setStartDate: (startDate: string) => void;
    showUrgency?: boolean;
}

export default function PublicDeliveryDetailsForm(props: Props) {
    const theme = useTheme();

    const startDate = useController({
        control: props.control,
        name: "startDate",
    }).field;
    const fieldDate = new Date(startDate.value);

    const urgency = useController({
        control: props.control,
        name: "urgency",
    }).field;

    const viewModel = usePublicDeliveryDetailsFormViewModel(
        fieldDate,
        props.setStartDate,
        urgency.value,
        props.setUrgency
    );

    // Stable data
    const now = useMemo(() => new Date(), []);
    const yesterday = useMemo(() => {
        const d = new Date();
        d.setDate(d.getDate() - 1);
        return d;
    }, []);

    return (
        <View style={{gap: 8}}>
            <Text style={[formStyle.label, theme.fonts.labelLarge]}>
                Date de livraison - Heure de mise à disposition par l'expéditeur
                <Text style={{color: theme.colors.error}}> *</Text>
            </Text>

            <View style={{flexDirection: 'row', flexWrap: 'wrap', columnGap: 24, rowGap: 16, alignItems: 'flex-end'}}>
                <View style={{flexDirection: 'row', alignItems: 'flex-end', gap: 8, flexShrink: 0}}>
                    <ThemedDateInput
                        style={{width: 300}}
                        testID="deliveryFormStartDateInput"
                        control={props.control}
                        name="startDate"
                        label="Date"
                        error={props.errors.startDate as FieldError | undefined}
                        validRange={{
                            startDate: now.getHours() < viewModel.lastHourToOrder ? yesterday : now,
                            endDate: undefined,
                            disabledDates: undefined
                        }}
                    />
                    <CompactDropdown
                        value={fieldDate.getHours().toString()}
                        options={viewModel.hoursOptions}
                        onSelect={(hours) => {
                            if (!hours) return;
                            const newDate = new Date(startDate.value);
                            newDate.setHours(parseInt(hours));
                            startDate.onChange(newDate.toISOString());
                        }}
                    />
                    <Text style={AppStyle.textStyle.h3}>h</Text>
                    <CompactDropdown
                        value={fieldDate.getMinutes().toString()}
                        options={viewModel.minutesOptions}
                        onSelect={(minutes) => {
                            if (!minutes) return;
                            const newDate = new Date(startDate.value);
                            newDate.setMinutes(parseInt(minutes));
                            startDate.onChange(newDate.toISOString());
                        }}
                    />
                    <Text style={AppStyle.textStyle.h3}>min</Text>
                </View>
                
                {props.showUrgency && (
                    <View style={{flexDirection: 'row', alignItems: 'flex-end', flexShrink: 0}}>
                        <View style={{width: 380}}>
                            <ThemedDropdownInput
                                testID="deliveryFormUrgencyInput"
                                control={props.control}
                                name="urgency"
                                error={props.errors.urgency as FieldError | undefined}
                                label="Urgence"
                                options={viewModel.urgencies}
                                required
                            />
                        </View>
                        <ThemedCheckboxInput
                            testID="deliveryFormInsulatedInput"
                            control={props.control}
                            name="insulatedBox"
                            error={props.errors.insulatedBox as FieldError | undefined}
                            label="Caisson isotherme"
                        />
                    </View>
                )}
            </View>
        </View>
    );
}