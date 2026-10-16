import {Button, Text} from "react-native-paper";
import GenericModal from "@/components/general/GenericModal";
import React from "react";

type Props = {
    visible: boolean;
    setVisible: (visible: boolean) => void;
    onDismiss: () => void;
}

export default function PublicDeliveryConfirmModal(props: Props) {
    
    return (
        <GenericModal
            visible={props.visible}>
            <Text>Votre commande a été créé avec succès. Vous allez recevoir un mail de confirmation.</Text>
            <Text>{"Pour plus d'informations, veuillez contacter le 06 49 66 21 16"}</Text>
            <Button mode="outlined" onPress={() => {
                props.setVisible(false);
                props.onDismiss();
            }
            }>Accueil</Button>
        </GenericModal>
    );
}