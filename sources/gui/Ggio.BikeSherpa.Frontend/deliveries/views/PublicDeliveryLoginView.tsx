import React from "react";
import {ActivityIndicator, Button, Divider, Text} from "react-native-paper";
import ThemedInput from "@/components/themed/ThemedInput";
import LoadingModal from "@/components/general/LoadingModal";
import BrandedCard from "@/components/general/BrandedCard";
import formStyle from "@/style/formStyle";
import usePublicDeliveryLoginViewModel from "@/deliveries/viewModel/usePublicDeliveryLoginViewModel";
import usePublicDeliveryModal from "@/deliveries/hooks/usePublicDeliveryModal";
import {StyleSheet, View} from "react-native";
import InputGroup from "@/components/themed/InputGroup";
import useCompanyInfo from "@/hooks/useCompanyInfo";

export default function PublicDeliveryLoginView() {
    const viewModel = usePublicDeliveryLoginViewModel();
    const {isLoadingModalVisible} = usePublicDeliveryModal();

    const companyInfo = useCompanyInfo();

    if (!companyInfo.label) {
        return (
            <View style={styles.loadingContainer}>
                <ActivityIndicator animating={true} size="large"/>
                <Text style={styles.loadingText}>Chargement en cours...</Text>
            </View>
        );
    }

    return (
        <>
            <View style={{height: '100%'}}>
                <BrandedCard
                    title={companyInfo.label}
                >
                    <InputGroup>
                        <ThemedInput
                            name={"email"}
                            control={viewModel.control}
                            label={"Email"}
                            placeholder={""}
                            error={viewModel.errors.email}
                            required
                            horizontal
                        />
                        <ThemedInput
                            name={"code"}
                            control={viewModel.control}
                            label={"Id Client"}
                            placeholder={""}
                            error={viewModel.errors.code}
                            required
                            horizontal
                        />
                    </InputGroup>
                    <Button
                        testID="formButton"
                        mode="outlined"
                        onPress={() => viewModel.handleSubmit()}
                        style={[formStyle.button, {width: "100%"}]}
                    >
                        <Text>Valider</Text>
                    </Button>
                    <Divider style={{width: "100%", marginBlock: 8}}/>
                    <Button
                        mode="outlined"
                        onPress={() => viewModel.proceedAsAnonymous()}
                        style={[formStyle.button, {width: "100%"}]}
                    >
                        <Text>{"Je n'ai pas d'identifiant"}</Text>
                    </Button>
                </BrandedCard>
            </View>
            {isLoadingModalVisible && (
                <LoadingModal visible={isLoadingModalVisible}/>
            )}
        </>
    );
}

const styles = StyleSheet.create({
    loadingContainer: {
        flex: 1,
        justifyContent: "center",
        alignItems: "center",
    },
    loadingText: {
        marginTop: 10,
    }
});