import { useEffect, useState } from "react";
import { Modal, View } from "react-native";
import { Button, Text, useTheme } from "react-native-paper";
import { SafeAreaProvider, SafeAreaView } from "react-native-safe-area-context";
import modalStyle from "@/style/modalStyle";

type ThemedConfirmationModalProps = {
    visible: boolean;
    title: string;
    confirmButton: () => void;
    cancelButton: () => void;
}


export default function ThemedConfirmationModal(props: ThemedConfirmationModalProps) {
    const theme = useTheme();
    const [modalVisible, setModalVisible] = useState(false);

    useEffect(() => {
        setModalVisible(props.visible);
    }, [props.visible]);

    return (
        <SafeAreaProvider>
            <SafeAreaView style={modalStyle.safeArea}>
                <Modal
                    animationType="slide"
                    transparent={true}
                    visible={modalVisible}>
                    <View style={[modalStyle.safeArea, { backgroundColor: theme.colors.onBackground }]}>
                        <View style={[modalStyle.modalContainer, { backgroundColor: theme.colors.background }]}>
                            <Text style={[modalStyle.title, theme.fonts.headlineSmall]}>{props.title}</Text>
                            <View style={modalStyle.buttonContainer}>
                                <Button labelStyle={theme.fonts.bodyLarge} onPress={props.confirmButton}>Oui</Button>
                                <Button labelStyle={theme.fonts.bodyLarge} onPress={props.cancelButton}>Non</Button>
                            </View>
                        </View>
                    </View>

                </Modal>
            </SafeAreaView>
        </SafeAreaProvider>
    )
}