import {View} from "react-native";
import ThemedInput from "@/components/themed/ThemedInput";
import {Button, Divider, Text} from "react-native-paper";
import formStyle from "@/style/formStyle";
import React from "react";
import usePublicDeliveryLoginFormViewModel from "@/deliveries/viewModel/usePublicDeliveryLoginFormViewModel";
import PublicDeliveryCustomer from "@/deliveries/models/PublicDeliveryCustomer";

type Props = {
    login: (customer?: PublicDeliveryCustomer) => void
}

export default function PublicDeliveryLoginForm(props: Props) {
    const viewModel = usePublicDeliveryLoginFormViewModel(props.login);

    return (
        <View style={{justifyContent: 'center', gap: 16, height: '100%', width: '75%', maxWidth: 500, margin: 'auto'}}>
            <ThemedInput
                name={"email"}
                control={viewModel.control}
                label={"Email"}
                placeholder={""}
                error={viewModel.errors.email}
                required
            />
            <ThemedInput
                name={"code"}
                control={viewModel.control}
                label={"Id Client"}
                placeholder={""}
                error={viewModel.errors.code}
                required
            />
            <Button
                testID="formButton"
                mode="outlined"
                onPress={() => viewModel.handleSubmit()}
                style={[formStyle.button, {width: '80%'}]}
            >
                <Text>Valider</Text>
            </Button>
            <Divider style={{width: '80%'}} />
            <Button
                mode="outlined"
                onPress={() => props.login()}
                style={[formStyle.button, {width: '80%'}]}
            >
                <Text>Je n'ai pas d'identifiant</Text>
            </Button>
        </View>
    );
}