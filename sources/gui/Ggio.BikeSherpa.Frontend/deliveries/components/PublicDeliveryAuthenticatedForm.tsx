import PublicDeliveryCustomer from "@/deliveries/models/PublicDeliveryCustomer";
import {Button, Divider, SegmentedButtons, Text} from "react-native-paper";
import PublicDeliveryStepForm from "@/steps/components/PublicDeliveryStepForm";
import PublicDeliveryDetailsForm from "@/deliveries/components/PublicDeliveryDetailsForm";
import PublicDeliveryStepDetailsForm from "@/steps/components/PublicDeliveryStepDetailsForm";
import PublicDeliveryPrice from "@/deliveries/components/PublicDeliveryPrice";
import usePublicDeliveryFormViewModel from "@/deliveries/viewModel/usePublicDeliveryFormViewModel";
import React from "react";
import {useController} from "react-hook-form";
import {ScrollView, View} from "react-native";
import AppStyle from "@/constants/AppStyle";
import formStyle from "@/style/formStyle";
import PublicDeliveryCustomerType from "@/deliveries/components/PublicDeliveryCustomerType";
import LoadingModal from "@/components/general/LoadingModal";
import PublicDeliveryErrorModal from "@/deliveries/components/PublicDeliveryErrorModal";

type Props = {
    customer: PublicDeliveryCustomer
}

export default function PublicDeliveryAuthenticatedForm(props: Props) {

    const viewModel = usePublicDeliveryFormViewModel(props.customer);

    const {field} = useController({
        control: viewModel.control,
        name: "pricingStrategy",
    });

    return (
        <ScrollView contentContainerStyle={{paddingInline: 16, paddingTop: 16, gap: 16, marginBottom: 64}}>
            <LoadingModal visible={viewModel.isLoading} />
            <PublicDeliveryErrorModal 
                visible={viewModel.showErrorModal} 
                setVisible={viewModel.setShowErrorModal}
                onDismiss={viewModel.goToLogin} />
            <View style={{flexDirection: "row", gap: 16, alignItems: "center"}}>
                <Text style={AppStyle.textStyle.h2}>Type de livraison</Text>
                <SegmentedButtons
                    value={field.value.toString()}
                    onValueChange={(value) => field.onChange(parseInt(value))}
                    buttons={viewModel.deliveryTypes.map(b => ({
                        ...b,
                        style: {width: 100}
                    }))}
                />
            </View>
            <Divider/>
            <PublicDeliveryStepForm
                control={viewModel.control}
                totalDistance={0}
                canAddStep={viewModel.deliveryTypes.length > 0 && field.value.toString() === viewModel.deliveryTypes[1].value}
            />
            <Divider/>
            <PublicDeliveryDetailsForm
                control={viewModel.control}
                errors={viewModel.errors}
                urgencies={viewModel.urgencies}
                packingSizes={viewModel.packingSizes}
            />
            <Divider/>
            <PublicDeliveryPrice
                price={0}
                priceWithTaxes={0}
            />
            <Divider/>
            <PublicDeliveryCustomerType
                customerType={viewModel.customerType}
                setCustomerType={viewModel.setCustomerType} />
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