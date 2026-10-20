import {Text} from "react-native-paper";
import {View} from "react-native";
import ThemedDateInput from "@/components/themed/ThemedDateInput";
import {Control, FieldError, FieldErrors, useController} from "react-hook-form";
import React, {useEffect, useMemo} from "react";
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
    const BOOKING_WINDOW_MONTHS = 6;
    const now = useMemo(() => new Date(), []);
    const yesterday = useMemo(() => {
        const d = new Date();
        d.setDate(d.getDate() - 1);
        return d;
    }, []);

    const isWeekend = (date: Date): boolean => {
        const day = date.getDay(); // 0 = Sunday, 6 = Saturday
        return day === 0 || day === 6;
    };

    const toNextWorkingDay = (date: Date): Date => {
        const result = new Date(date);
        while (isWeekend(result))
            result.setDate(result.getDate() + 1);
        return result;
    };

    const getWeekendDates = (from: Date, to: Date): Date[] => {
        const weekends: Date[] = [];
        const cursor = new Date(from);
        cursor.setHours(0, 0, 0, 0);

        while (cursor <= to) {
            if (isWeekend(cursor))
                weekends.push(new Date(cursor));
            cursor.setDate(cursor.getDate() + 1);
        }
        return weekends;
    };

    const buildValidRange = (earliest: Date, today: Date) => {
        const latest = new Date(today);
        latest.setMonth(latest.getMonth() + BOOKING_WINDOW_MONTHS);

        return {
            startDate: earliest,
            endDate: latest,
            disabledDates: getWeekendDates(earliest, latest)
        };
    };

    const validRange = useMemo(
        () => buildValidRange(
            now.getHours() < viewModel.lastHourToOrder ? yesterday : now,
            now
        ),
        [now, yesterday, viewModel.lastHourToOrder] // eslint-disable-line react-hooks/exhaustive-deps
    );

    // If the current value is a Saturday or Sunday, move it to the next Monday
    useEffect(() => {
        const current = new Date(startDate.value);
        if (isWeekend(current))
            startDate.onChange(toNextWorkingDay(current).toISOString());
    }, [startDate.value]); // eslint-disable-line react-hooks/exhaustive-deps

    const cell = {
        flexBasis: '48%',
        flexGrow: 0,
        flexShrink: 1,
        minWidth: 300,
        maxWidth: '100%',
        alignItems: 'center',
    } as const;

    return (
        <View style={{gap: 8}}>
            <Text style={[AppStyle.textStyle.h2, {textAlign: 'center'}]}>
                Créneau de livraison
            </Text>
            <View style={{
                flexDirection: 'row',
                flexWrap: 'wrap',
                justifyContent: 'center',
                columnGap: 8,
                rowGap: 16,
                width: '100%',
            }}>
                <View style={cell}>
                    <ThemedDateInput
                        style={{width: 300, minWidth: 250, maxWidth: '100%'}}
                        testID="deliveryFormStartDateInput"
                        control={props.control}
                        name="startDate"
                        label="Date de livraison"
                        error={props.errors.startDate as FieldError | undefined}
                        validRange={validRange}
                    />
                </View>
                <View style={cell}>
                    <Text>
                        Heure de mise à disposition
                    </Text>
                    <View style={{flexDirection: 'row', alignItems: 'center', gap: 8}}>
                        <View style={{flexDirection: 'row', alignItems: 'center', gap: 4}}>
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
                        </View>
                        <View style={{flexDirection: 'row', alignItems: 'center', gap: 4}}>
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
                    </View>
                    <Text style={{fontStyle: 'italic'}}>
                        {"L'enlèvement pourra avoir lieu A PARTIR de cet horaire"}
                    </Text>
                </View>

                {props.showUrgency && (
                    <>
                        <View style={cell}>
                            <View style={{width: 380, maxWidth: '100%'}}>
                                <ThemedDropdownInput
                                    testID="deliveryFormUrgencyInput"
                                    control={props.control}
                                    name="urgency"
                                    error={props.errors.urgency as FieldError | undefined}
                                    label="Durée du créneau"
                                    options={viewModel.urgencies}
                                    required
                                    style={{width: '100%'}}
                                />
                            </View>
                        </View>
                        <View style={cell}>
                            <ThemedCheckboxInput
                                testID="deliveryFormInsulatedInput"
                                control={props.control}
                                name="insulatedBox"
                                error={props.errors.insulatedBox as FieldError | undefined}
                                label="Caisson isotherme"
                                style={{width: '100%', alignItems: 'center'}}
                            />
                        </View>
                    </>
                )}
            </View>
        </View>
    );
}