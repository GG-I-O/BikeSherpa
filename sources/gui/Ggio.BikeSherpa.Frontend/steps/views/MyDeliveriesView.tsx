import { navigate } from "expo-router/build/global-state/routing";
import React from "react";
import { View } from "react-native";
import StepCardList from "../components/StepCardList";
import { useTheme } from "react-native-paper";
import useMyDeliveriesViewModel from "@/steps/viewModel/useMyDeliveriesViewModel";
import {DatePickerInput} from "react-native-paper-dates";
import {stepDatePickerStore$} from "@/steps/store/StepDatePickerStore";
import {useValue} from "@legendapp/state/react";

export function MyDeliveriesView() {
    const theme = useTheme();

    const viewModel = useMyDeliveriesViewModel();

    const selectedDate = useValue(stepDatePickerStore$.date);

    return (
        <>
            <View
                style={{
                    flexDirection: 'row', flexGrow: 0, flexShrink: 0,
                    backgroundColor: theme.colors.background, padding: 8
                }}
            >
                <DatePickerInput
                    locale={"fr"}
                    inputMode={"start"}
                    onChange={(date: Date | undefined): void => stepDatePickerStore$.date.set(date)}
                    value={selectedDate}
                />
            </View>
            <StepCardList
                steps={viewModel.steps}
                onCardPress={
                    (step) => navigate({
                        pathname: '/(tabs)/myDeliveries/[stepId]',
                        params: { stepId: step.id }
                    })
                }
                style={{ alignItems: 'center' }}
            />
        </>
    );
}