import {Text} from "react-native-paper";
import {View} from "react-native";
import ThemedDateInput from "@/components/themed/ThemedDateInput";
import {Control, FieldError, FieldErrors, useController} from "react-hook-form";
import {DropdownOptions} from "@/models/DropdownOptions";
import formStyle from "@/style/formStyle";
import React from "react";
import {Dropdown} from "react-native-paper-dropdown";
import ThemedDropdownInput from "@/components/themed/ThemedDropdownInput";
import ThemedCheckboxInput from "@/components/themed/ThemedCheckboxInput";
import {PublicDeliveryFormValues} from "@/deliveries/models/zod/publicDeliveryFormBaseSchema";
import AppStyle from "@/constants/AppStyle";
import DateToolbox from "@/services/DateToolbox";

type Props = {
    control: Control<PublicDeliveryFormValues>;
    errors: FieldErrors;
    urgencies: DropdownOptions[];
    packingSizes: DropdownOptions[];
}

export default function PublicDeliveryDetailsForm(props: Props) {

    // Il faudrait sûrement un endpoint pour les heures qui limitent les choix
    const now = new Date();
    const yesterday = new Date(now);
    yesterday.setDate(now.getDate() - 1);

    // Time
    const {field} = useController({
        control: props.control,
        name: "startDate",
    });

    const fieldDate = field.value ? new Date(field.value) : new Date();
    const isFieldToday = DateToolbox.getFormattedDateFromISO(now.toISOString()) === DateToolbox.getFormattedDateFromISO(fieldDate.toISOString());

    let hoursOptions: DropdownOptions[] = [];
    const startHours = isFieldToday ? now.getHours() + 1 : 8;
    for (let i = startHours; i < 17; i++) {
        hoursOptions.push({label: i.toString(), value: i.toString()});
    }

    let minutesOptions: DropdownOptions[] = [];
    for (let i = 0; i < 60; i += 15) {
        minutesOptions.push({label: i.toString(), value: i.toString()});
    }

    // Urgencies - Il faudra avoir ces horaires quelque part
    let urgencies = props.urgencies;
    if (isFieldToday) {
        urgencies = urgencies.filter((urgency, index) => {
            if (index === 2) return true;

            if (index === 1) {
                return now.getHours() <= 14;
            }
            if (index === 0) {
                return now.getHours() <= 12;
            }
        });
    }

    return (
        <View style={{gap: 16}}>
            <View style={[formStyle.intputContainer, {flexDirection: 'row', alignItems: 'flex-end'}]}>
                <ThemedDateInput
                    style={{marginRight: 16}}
                    testID="deliveryFormStartDateInput"
                    control={props.control}
                    name="startDate"
                    error={props.errors.startDate as FieldError | undefined}
                    label="Date de livraison - Heure de mise à disposition par l'expéditeur"
                    validRange={{
                        startDate: now.getHours() < 15 ? yesterday : now,
                        endDate: undefined,
                        disabledDates: undefined
                    }}
                    required
                />
                <Dropdown
                    value={fieldDate.getHours().toString()}
                    onSelect={(hours) => {
                        if (!hours) return;
                        let newDate: Date = new Date(field.value);
                        newDate.setHours(parseInt(hours));
                        field.onChange(newDate.toISOString());

                    }}
                    mode='outlined'
                    options={hoursOptions}
                />
                <Text style={AppStyle.textStyle.h3}> h </Text>
                <Dropdown
                    value={fieldDate.getMinutes().toString()}
                    onSelect={(minutes) => {
                        if (!minutes) return;
                        let newDate: Date = new Date(field.value);
                        newDate.setMinutes(parseInt(minutes));
                        field.onChange(newDate.toISOString());
                    }}
                    mode='outlined'
                    options={minutesOptions}
                />
                <Text style={AppStyle.textStyle.h3}> min </Text>
            </View>
            <ThemedDropdownInput
                testID="deliveryFormUrgencyInput"
                control={props.control}
                name="urgency"
                error={props.errors.urgency as FieldError | undefined}
                label="Urgence"
                options={urgencies}
                required
            />
            <ThemedDropdownInput
                testID="deliveryFormPackingSizeInput"
                control={props.control}
                name="packingSize"
                error={props.errors.packingSize as FieldError | undefined}
                label="Tranche - selon poids ET plus grande dimension du colis"
                options={props.packingSizes}
                required
            />
            <ThemedCheckboxInput
                testID="deliveryFormInsulatedInput"
                control={props.control}
                name="insulatedBox"
                error={props.errors.insulatedBox as FieldError | undefined}
                label="Caisson isotherme"
                required
            />
        </View>
    );
}