import {dismissTo} from "expo-router/build/global-state/routing";
import {publicDeliveryStore$} from "@/deliveries/store/publicDeliveryStore";
import {useSelector} from "@legendapp/state/react";

export default function usePublicDeliveryModal() {
    const isLoadingModalVisible = useSelector(() => publicDeliveryStore$.isLoadingModalVisible.get());
    const isErrorModalVisible = useSelector(() => publicDeliveryStore$.isErrorModalVisible.get());
    const isConfirmModalVisible = useSelector(() => publicDeliveryStore$.isConfirmModalVisible.get());

    const setIsLoadingModalVisible = (value: boolean) =>
        publicDeliveryStore$.isLoadingModalVisible.set(value);

    const setIsErrorModalVisible = (value: boolean) =>
        publicDeliveryStore$.isErrorModalVisible.set(value);

    const onErrorModalDismiss = () => dismissTo("/newDelivery");

    const setIsConfirmModalVisible = (value: boolean) =>
        publicDeliveryStore$.isConfirmModalVisible.set(value);

    const onConfirmModalDismiss = () => dismissTo("/newDelivery");

    return {
        isLoadingModalVisible,
        setIsLoadingModalVisible,
        isErrorModalVisible,
        setIsErrorModalVisible,
        onErrorModalDismiss,
        isConfirmModalVisible,
        setIsConfirmModalVisible,
        onConfirmModalDismiss
    };
}