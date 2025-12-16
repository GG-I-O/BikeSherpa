import { navigate } from "expo-router/build/global-state/routing";
import { useEffect, useState } from "react";
import { View } from "react-native";
import useDeliveryViewModel from "../../deliveries/viewModel/DeliveryViewModel";
import { Step } from "../models/Step";
import StepCardList from "../components/StepCardList";
import { SegmentedButtons, useTheme } from "react-native-paper";

export function MyDeliveryView() {
    const theme = useTheme();
    // Data
    const [steps, setSteps] = useState<Step[]>([]);

    const viewModel = useDeliveryViewModel();

    const [dateFilter, setDateFilter] = useState<string>('1');

    useEffect(() => {
        setSteps(viewModel.getFilteredStepList(dateFilter, 'NONE'));
    }, [viewModel, dateFilter]);

    return (
        <>
            <View
                style={{
                    flexDirection: 'row', flexGrow: 0, flexShrink: 0,
                    backgroundColor: theme.colors.background, padding: 8
                }}
            >
                <SegmentedButtons
                    style={{ height: "auto", flex: 1, alignItems: "center" }}
                    value={dateFilter}
                    onValueChange={setDateFilter}
                    buttons={[
                        {
                            value: "1",
                            label: "Aujourd’hui"
                        },
                        {
                            value: "2",
                            label: "Demain"
                        }
                    ]}
                />
            </View>
            <StepCardList
                steps={steps}
                onCardPress={
                    (step) => navigate({
                        pathname: '/(tabs)/(myDeliveries)/[stepId]',
                        params: { stepId: step.id }
                    })
                }
                style={{ alignItems: 'center' }}
            />
        </>
    );
}