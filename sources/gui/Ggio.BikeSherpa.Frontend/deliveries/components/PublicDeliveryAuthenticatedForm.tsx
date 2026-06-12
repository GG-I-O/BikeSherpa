import PublicDeliveryCustomer from "@/deliveries/models/PublicDeliveryCustomer";
import {Button, Divider, SegmentedButtons, Text} from "react-native-paper";
import PublicDeliveryStepForm from "@/steps/components/PublicDeliveryStepForm";
import PublicDeliveryDetailsForm from "@/deliveries/components/PublicDeliveryDetailsForm";
import PublicDeliveryPrice from "@/deliveries/components/PublicDeliveryPrice";
import usePublicDeliveryFormViewModel from "@/deliveries/viewModel/usePublicDeliveryFormViewModel";
import React from "react";
import {useController} from "react-hook-form";
import {ScrollView, View} from "react-native";
import AppStyle from "@/constants/AppStyle";
import formStyle from "@/style/formStyle";
import PublicDeliveryCustomerType from "@/deliveries/components/PublicDeliveryCustomerType";

type Props = {
    customer: PublicDeliveryCustomer
}

export default function PublicDeliveryAuthenticatedForm(props: Props) {

    const viewModel = usePublicDeliveryFormViewModel(props.customer);

    const {field} = useController({
        control: viewModel.control,
        name: "pricingStrategy",
    });

    const deliveryType = useController({
        control: viewModel.control,
        name: "pricingStrategy",
    }).field;
    const isSimpleDelivery = 
        viewModel.deliveryTypes.length > 0 &&
        deliveryType.value.toString() === viewModel.deliveryTypes[0].value;

    return (
        <ScrollView contentContainerStyle={{paddingInline: 16, paddingTop: 16, gap: 16, marginBottom: 64}}>
            <View style={{flexDirection: "row", gap: 16, alignItems: "center"}}>
                <Text style={AppStyle.textStyle.h2}>Type de livraison</Text>
                <SegmentedButtons
                    value={field.value.toString()}
                    onValueChange={(value) => field.onChange(parseInt(value))}
                    buttons={viewModel.deliveryTypes.map(b => ({
                        ...b,
                        style: {width: 250}
                    }))}
                />
            </View>
            <Divider/>
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
                canAddStep={viewModel.deliveryTypes.length > 0 && field.value.toString() === viewModel.deliveryTypes[1].value}
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