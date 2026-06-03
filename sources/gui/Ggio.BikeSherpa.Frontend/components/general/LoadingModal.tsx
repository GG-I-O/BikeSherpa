import {ActivityIndicator, Button, Text, useTheme} from "react-native-paper";
import {useEffect, useState} from "react";
import {SafeAreaProvider, SafeAreaView} from "react-native-safe-area-context";
import modalStyle from "@/style/modalStyle";
import {Modal, View} from "react-native";

type LoadingModalProps = {
    visible: boolean;
}

export default function LoadingModal(props: LoadingModalProps) {
    const theme = useTheme();
    const [modalVisible, setModalVisible] = useState(false);

    useEffect(() => {
        setModalVisible(props.visible);
    }, [props.visible]);

    return (
        <SafeAreaView style={modalStyle.safeArea}>
            <Modal
                animationType="slide"
                transparent={true}
                visible={modalVisible}>
                <View style={[modalStyle.safeArea, {backgroundColor: theme.colors.onBackground}]}>
                    <ActivityIndicator animating color={theme.colors.background} size="large"/>
                </View>
            </Modal>
        </SafeAreaView>
    )
}