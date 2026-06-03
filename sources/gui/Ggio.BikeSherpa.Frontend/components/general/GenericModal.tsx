import {useTheme} from "react-native-paper";
import {PropsWithChildren, useEffect, useState} from "react";
import {SafeAreaProvider, SafeAreaView} from "react-native-safe-area-context";
import modalStyle from "@/style/modalStyle";
import {Modal, View} from "react-native";

type Props = {
    visible: boolean;
}

export default function GenericModal(props: PropsWithChildren<Props>) {
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
                    transparent
                    visible={modalVisible}
                >
                    <View style={[modalStyle.safeArea, { backgroundColor: 'rgba(0, 0, 0, 0.75)' }]}>
                        <View style={[modalStyle.modalContainer, { backgroundColor: theme.colors.background }]}>
                            {props.children}
                        </View>
                    </View>

                </Modal>
            </SafeAreaView>
        </SafeAreaProvider>
    );
}