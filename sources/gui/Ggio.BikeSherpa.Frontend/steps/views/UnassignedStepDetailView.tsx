import {useLocalSearchParams} from "expo-router";
import {Button, Text, useTheme} from "react-native-paper";
import React, {useState} from "react";
import useUnassignedStepDetailViewModel from "@/steps/viewModel/useUnassignedStepDetailViewModel";
import StepDetailView from "@/steps/views/StepDetailView";
import {View} from "react-native";
import {navigate} from "expo-router/build/global-state/routing";

export default function UnassignedStepDetailView() {
    const theme = useTheme();

    const {stepId} = useLocalSearchParams<{ stepId: string }>();

    const viewModel = useUnassignedStepDetailViewModel();

    const [pressed, setPressed] = useState<boolean>(false);

    return (
        <>
            {!pressed &&
                <View style={{width: '100%', backgroundColor: theme.colors.background, paddingInline: 8}}>
                    <Button
                        buttonColor={theme.colors.background}
                        mode="outlined"
                        onPress={() => {
                            setPressed(true);
                            viewModel.assignMyself(stepId);
                            navigate({
                                pathname: '/(tabs)/unassignedDeliveries'
                            });
                        }}
                    >
                        <Text>Prendre la course</Text>
                    </Button>
                </View>
            }
            <StepDetailView/>
        </>
    );
}