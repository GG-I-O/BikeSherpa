import React, {useCallback, useEffect, useState} from "react";
import { View } from "react-native";
import { Button, Text } from 'react-native-paper';
import { TimePickerModal } from 'react-native-paper-dates';
import { SafeAreaProvider } from "react-native-safe-area-context";

type Props = {
    hours: number,
    minutes: number,
    onConfirm: ({ hours, minutes }: { hours: number, minutes: number }) => void,
    onOpen?: () => void,
    onClose?: () => void
}

export default function TimePickerInput({ hours, minutes, onConfirm, onOpen, onClose }: Props) {
    const [visible, setVisible] = useState(false);
    
    const [localHours, setLocalHours] = useState(hours);
    const [localMinutes, setLocalMinutes] = useState(minutes);
    
    useEffect(() => {
        setLocalHours(hours);
        setLocalMinutes(minutes);
    }, [hours, minutes]);

    const onDismiss = useCallback(() => {
        setVisible(false);
        onClose?.();
    }, [onClose]);

    const onConfirmPress = useCallback((hoursAndMinutes: {hours: number, minutes: number}) => {
        setLocalHours(hoursAndMinutes.hours);
        setLocalMinutes(hoursAndMinutes.minutes);
        onConfirm({ hours: hoursAndMinutes.hours, minutes: hoursAndMinutes.minutes });
        setVisible(false);
        onClose?.();
    }, [localHours, localMinutes, onConfirm, onClose]);

    return (
        <SafeAreaProvider>
            <View style={{ justifyContent: 'center', flex: 1, alignItems: 'flex-start' }}>
                <Button
                    onPress={() => {
                        setVisible(true);
                        onOpen?.();
                    }}
                    uppercase={false}
                    mode="outlined"
                >
                    <Text style={{ marginInline: 0 }}>
                        {`${localHours.toString().padStart(2, '0')}:${localMinutes.toString().padStart(2, '0')}`}
                    </Text>
                </Button>
                <TimePickerModal
                    visible={visible}
                    onDismiss={onDismiss}
                    onConfirm={onConfirmPress}
                    hours={localHours}
                    minutes={localMinutes}
                />
            </View>
        </SafeAreaProvider>
    );
}