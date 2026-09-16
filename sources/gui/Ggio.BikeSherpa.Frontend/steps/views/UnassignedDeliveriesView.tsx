import { navigate } from "expo-router/build/global-state/routing";
import React from "react";
import { View } from "react-native";
import StepCardList from "../components/StepCardList";
import { useTheme } from "react-native-paper";
import {DatePickerInput} from "react-native-paper-dates";
import useUnassignedDeliveriesViewModel from "@/steps/viewModel/useUnassignedDeliveriesViewModel";
import {stepDatePickerStore$} from "@/steps/store/StepDatePickerStore";
import { useValue } from "@legendapp/state/react";

export function UnassignedDeliveriesView() {
    const theme = useTheme();

    const viewModel = useUnassignedDeliveriesViewModel();

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
                        pathname: '/(tabs)/unassignedDeliveries/[stepId]',
                        params: { stepId: step.id }
                    })
                }
                style={{ alignItems: 'center' }}
            />
        </>
    );
}