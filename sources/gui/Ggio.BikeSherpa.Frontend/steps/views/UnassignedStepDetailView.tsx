import {useLocalSearchParams} from "expo-router";
import {Button, Text, useTheme} from "react-native-paper";
import React, {useState} from "react";
import useUnassignedStepDetailViewModel from "@/steps/viewModel/useUnassignedStepDetailViewModel";
import StepDetailView from "@/steps/views/StepDetailView";

export default function UnassignedStepDetailView() {
    const theme = useTheme();

    const {stepId} = useLocalSearchParams<{ stepId: string }>();

    const viewModel = useUnassignedStepDetailViewModel();

    const [pressed, setPressed] = useState<boolean>(false);

    return (
        <>
            {!pressed &&
                <Button
                    buttonColor={theme.colors.background}
                    style={{marginInline: 8}}
                    mode="outlined"
                    onPress={() => {
                        setPressed(true);
                        viewModel.assignMyself(stepId);
                    }}
                >
                    <Text>M'assigner</Text>
                </Button>
            }
            <StepDetailView />
        </>
    );
}