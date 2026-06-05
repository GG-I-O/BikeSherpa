import {Button, Text} from "react-native-paper";
import GenericModal from "@/components/general/GenericModal";
import React from "react";

type Props = {
    visible: boolean;
    setVisible: (visible: boolean) => void;
    onDismiss: () => void;
}

export default function PublicDeliveryErrorModal(props: Props) {
    
    return (
        <GenericModal
            visible={props.visible}>
            <Text>Une erreur est survenue lors de la création de la livraison</Text>
            <Text>{"Pour plus d'informations, veuillez contacter le 06 49 66 21 16"}</Text>
            <Button mode="outlined" onPress={() => {
                props.setVisible(false);
                props.onDismiss();
            }
            }>Retour</Button>
        </GenericModal>
    )
}