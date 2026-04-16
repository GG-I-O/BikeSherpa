import {Control, useController} from "react-hook-form";
import React from "react";
import {Button, Text, useTheme} from "react-native-paper";
import {View} from "react-native";
import formStyle from "@/style/formStyle";
import DeliveryTypeIcon from "@/deliveries/components/DeliveryTypeIcon";
import {StepType} from "@/steps/models/StepType";

interface CustomStepTypeInputProps {
    name: string;
    control: Control<any>;
    testID?: string;
}

const StepTypeInput: React.FC<CustomStepTypeInputProps> = (
    {
        name,
        control,
        testID
    }
) => {
    const theme = useTheme();

    const {field} = useController({
        control,
        name,
    });

    return (
        <View style={formStyle.intputContainer}>
            <Button
                style={{minWidth: 0}}
                mode='outlined'
                onPress={()=> field.onChange(field.value === 0 ? StepType.Drop : StepType.PickUp)}
                testID={testID}
            >
                {/* Wrap the Icon into a text to be able to reduce margin */}
                <Text style={{marginInline: 0}}>
                    <DeliveryTypeIcon type={field.value === 0 ? StepType.PickUp : StepType.Drop}/>
                </Text>
            </Button>
        </View>
    );
}

export default StepTypeInput;