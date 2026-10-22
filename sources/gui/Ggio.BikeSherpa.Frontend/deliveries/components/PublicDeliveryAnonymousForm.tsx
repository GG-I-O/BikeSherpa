import {Button, Divider, Text, useTheme} from "react-native-paper";
import PublicDeliveryStepForm from "@/steps/components/PublicDeliveryStepForm";
import PublicDeliveryDetailsForm from "@/deliveries/components/PublicDeliveryDetailsForm";
import PublicDeliveryPrice from "@/deliveries/components/PublicDeliveryPrice";
import usePublicDeliveryFormViewModel from "@/deliveries/viewModel/usePublicDeliveryFormViewModel";
import formStyle from "@/style/formStyle";
import React from "react";
import {useController} from "react-hook-form";
import {KeyboardAwareScrollView} from "react-native-keyboard-controller";

export default function PublicDeliveryAnonymousForm() {
    const theme = useTheme();
    const viewModel = usePublicDeliveryFormViewModel();

    const deliveryType = useController({
        control: viewModel.control,
        name: "pricingStrategy",
    }).field;

    const isSimpleDelivery =
        viewModel.deliveryTypes.length > 0 &&
        deliveryType.value.toString() === viewModel.deliveryTypes[0].value;

    return (
        <KeyboardAwareScrollView
            bottomOffset={32}
            keyboardShouldPersistTaps={'handled'}
            contentContainerStyle={{paddingInline: 16, paddingTop: 16, paddingBottom: 64, gap: 16}}
        >
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
            <PublicDeliveryPrice
                control={viewModel.control}
                price={viewModel.estimatedPrice}
                priceWithTaxes={viewModel.estimatedPriceWithTaxes}
            />
            <Divider/>
            <Button
                testID="formButton"
                mode="outlined"
                onPress={() => viewModel.handleSubmit()}
                style={[formStyle.button, {backgroundColor: theme.colors.secondaryContainer}]}
            >
                <Text
                    testID="buttonName"
                    style={{fontWeight: 'bold', color: theme.colors.onSecondaryContainer}}
                >
                    Passer à la facturation (1/2)
                </Text>
            </Button>
        </KeyboardAwareScrollView>
    );
}