import {Button, Divider, Text} from "react-native-paper";
import PublicDeliveryStepForm from "@/steps/components/PublicDeliveryStepForm";
import PublicDeliveryDetailsForm from "@/deliveries/components/PublicDeliveryDetailsForm";
import PublicDeliveryPrice from "@/deliveries/components/PublicDeliveryPrice";
import PublicDeliveryCustomerForm from "@/deliveries/components/PublicDeliveryCustomerForm";
import usePublicDeliveryFormViewModel from "@/deliveries/viewModel/usePublicDeliveryFormViewModel";
import {ScrollView} from "react-native";
import formStyle from "@/style/formStyle";
import React from "react";
import PublicDeliveryCustomerType from "@/deliveries/components/PublicDeliveryCustomerType";
import {useController} from "react-hook-form";

export default function PublicDeliveryAnonymousForm() {

    const viewModel = usePublicDeliveryFormViewModel();

    const deliveryType = useController({
        control: viewModel.control,
        name: "pricingStrategy",
    }).field;
    
    const isSimpleDelivery = 
        viewModel.deliveryTypes.length > 0 &&
        deliveryType.value.toString() === viewModel.deliveryTypes[0].value;
    
    return (
        <ScrollView contentContainerStyle={{paddingInline: 16, paddingTop: 16, gap: 16, marginBottom: 64}}>
            <PublicDeliveryCustomerType
                customerType={viewModel.customerType}
                setCustomerType={viewModel.setCustomerType}/>
            <Divider/>
            <PublicDeliveryStepForm
                control={viewModel.control}
                errors={viewModel.errors}
                packingSizes={viewModel.packingSizes}
                customerType={viewModel.customerType}
                totalDistance={viewModel.estimatedDistance}
            />
            <Divider/>
            <PublicDeliveryDetailsForm
                control={viewModel.control}
                errors={viewModel.errors}
                setUrgency={viewModel.setUrgency}
                setStartDate={viewModel.setStartDate}
                showUrgency={isSimpleDelivery}
            />
            <Divider/>
            {isSimpleDelivery && (
                <>
                    <PublicDeliveryPrice
                        control={viewModel.control}
                        price={viewModel.estimatedPrice}
                        priceWithTaxes={viewModel.estimatedPriceWithTaxes}
                    />
                    <Divider/>
                </>
            )}
            <PublicDeliveryCustomerForm
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