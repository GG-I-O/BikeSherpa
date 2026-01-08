import { useState } from "react";
import { Snackbar } from "react-native-paper";

type AppSnackbarProps = {
    visible: boolean;
    text: string;
    onDismiss: () => void;
}

export default function AppSnackbar(props: AppSnackbarProps) {
    return <Snackbar
        testID="AppSnackbar"
        duration={2000}
        visible={props.visible}
        onDismiss={() => props.onDismiss()}
        action={{
            label: 'fermer',
            onPress: () => props.onDismiss()
        }}
    >
        {props.text}
    </Snackbar>
}