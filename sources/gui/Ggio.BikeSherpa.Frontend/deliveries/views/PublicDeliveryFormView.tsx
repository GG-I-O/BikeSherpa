import PublicDeliveryAnonymousForm from "@/deliveries/components/PublicDeliveryAnonymousForm";
import PublicDeliveryAuthenticatedForm from "@/deliveries/components/PublicDeliveryAuthenticatedForm";
import {publicDeliveryStore$} from "@/deliveries/store/publicDeliveryStore";
import {Stack} from "expo-router";
import React from "react";
import PublicDeliveryErrorModal from "@/deliveries/components/PublicDeliveryErrorModal";
import usePublicDeliveryModal from "@/deliveries/hooks/usePublicDeliveryModal";

export default function PublicDeliveryFormView() {
    const customer = publicDeliveryStore$.customer.get();
    const isAnonymous = publicDeliveryStore$.isAnonymous.get();

    const {isErrorModalVisible, setIsErrorModalVisible, onErrorModalDismiss} = usePublicDeliveryModal();
    
    return (
        <>
            <PublicDeliveryErrorModal
                visible={isErrorModalVisible}
                setVisible={setIsErrorModalVisible}
                onDismiss={onErrorModalDismiss} />
            { customer && !isAnonymous ? (
                <>
                    <Stack.Screen options={{title: `${customer.name} - Nouvelle livraison`}}/>
                    <PublicDeliveryAuthenticatedForm customer={customer}/>
                </>
            ) : (
                <PublicDeliveryAnonymousForm/>
            )}
        </>
    );
}
