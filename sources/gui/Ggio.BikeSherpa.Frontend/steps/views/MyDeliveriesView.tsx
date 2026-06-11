import { navigate } from "expo-router/build/global-state/routing";
import React from "react";
import { View } from "react-native";
import StepCardList from "../components/StepCardList";
import { useTheme } from "react-native-paper";
import useMyDeliveriesViewModel from "@/steps/viewModel/useMyDeliveriesViewModel";
import {DatePickerInput} from "react-native-paper-dates";

export function MyDeliveriesView() {
    const theme = useTheme();

    const viewModel = useMyDeliveriesViewModel();

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
                    onChange={(date: Date | undefined): void => viewModel.setDatePicker(date)}
                    value={viewModel.datePicker}
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