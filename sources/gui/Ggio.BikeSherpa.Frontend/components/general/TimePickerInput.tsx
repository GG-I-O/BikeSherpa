import React, { useCallback, useState } from "react";
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

    const onDismiss = useCallback(() => {
        setVisible(false);
        onClose?.();
    }, [setVisible]);

    const onConfirmPress = useCallback(({ hours, minutes }: { hours: number, minutes: number }) => {
        onConfirm({ hours, minutes });
        setVisible(false);
        onClose?.();
    }, [setVisible]);

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
                        {`${hours.toString().padStart(2, '0')}:${minutes.toString().padStart(2, '0')}`}
                    </Text>
                </Button>
                <TimePickerModal
                    visible={visible}
                    onDismiss={onDismiss}
                    onConfirm={onConfirmPress}
                    hours={hours}
                    minutes={minutes}
                />
            </View>
        </SafeAreaProvider>
    );
}