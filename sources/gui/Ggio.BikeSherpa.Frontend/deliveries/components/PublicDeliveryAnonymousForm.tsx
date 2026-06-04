import {Button, Divider, Text} from "react-native-paper";
import PublicDeliveryStepForm from "@/steps/components/PublicDeliveryStepForm";
import PublicDeliveryDetailsForm from "@/deliveries/components/PublicDeliveryDetailsForm";
import PublicDeliveryStepDetailsForm from "@/steps/components/PublicDeliveryStepDetailsForm";
import PublicDeliveryPrice from "@/deliveries/components/PublicDeliveryPrice";
import PublicDeliveryCustomerForm from "@/deliveries/components/PublicDeliveryCustomerForm";
import usePublicDeliveryFormViewModel from "@/deliveries/viewModel/usePublicDeliveryFormViewModel";
import {ScrollView} from "react-native";
import formStyle from "@/style/formStyle";
import React from "react";
import PublicDeliveryCustomerType from "@/deliveries/components/PublicDeliveryCustomerType";
import LoadingModal from "@/components/general/LoadingModal";
import PublicDeliveryErrorModal from "@/deliveries/components/PublicDeliveryErrorModal";

export default function PublicDeliveryAnonymousForm() {

    const viewModel = usePublicDeliveryFormViewModel();

    return (
        <ScrollView contentContainerStyle={{paddingInline: 16, paddingTop: 16, gap: 16, marginBottom: 64}}>
            <LoadingModal visible={viewModel.isLoading} />
            <PublicDeliveryErrorModal
                visible={viewModel.showErrorModal}
                setVisible={viewModel.setShowErrorModal}
                onDismiss={viewModel.goToLogin} />
            <PublicDeliveryStepForm
                control={viewModel.control}
                totalDistance={viewModel.estimatedDistance}
            />
            <Divider />
            <PublicDeliveryDetailsForm
                control={viewModel.control}
                errors={viewModel.errors}
                urgencies={viewModel.urgencies}
                packingSizes={viewModel.packingSizes}
            />
            <Divider />
            <PublicDeliveryPrice
                price={viewModel.estimatedPrice}
                priceWithTaxes={viewModel.estimatedPriceWithTaxes}
            />
            <Divider />
            <PublicDeliveryCustomerType 
                customerType={viewModel.customerType}
                setCustomerType={viewModel.setCustomerType} />
            <Divider />
            <PublicDeliveryCustomerForm
                control={viewModel.control}
                errors={viewModel.errors}
                customerType={viewModel.customerType}
            />
            <Divider />
            <PublicDeliveryStepDetailsForm
                control={viewModel.control}
                errors={viewModel.errors}
                customerType={viewModel.customerType}
            />
            <Button
                testID="formButton"
                mode="outlined"
                onPress={() => viewModel.handleSubmit()}
                style={formStyle.button}
            >
                <Text testID="buttonName">Valider</Text>
            </Button>
        </ScrollView>
    );
}