import { View } from "react-native";
import { Portal, Provider } from "react-native-paper";
import AppSnackbar from "../components/AppSnackbar";
import useAppSnackbarViewModel from "../viewModel/useAppSnackbarViewModel";

export default function AppSnackbarView() {
    const { visibility, message, onDismiss } = useAppSnackbarViewModel();
    return (
        <View style={{ position: "absolute", zIndex: 9999, bottom: 0, width: "100%" }}>
            <Provider>
                <Portal>
                    <AppSnackbar
                        visible={visibility}
                        text={message}
                        onDismiss={onDismiss} />
                </Portal>
            </Provider>
        </View>
    );
}