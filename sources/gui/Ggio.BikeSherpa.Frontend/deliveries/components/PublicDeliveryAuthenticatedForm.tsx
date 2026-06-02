import PublicDeliveryCustomer from "@/deliveries/models/PublicDeliveryCustomer";
import {SegmentedButtons} from "react-native-paper";
import PublicDeliveryStepForm from "@/steps/components/PublicDeliveryStepForm";
import PublicDeliveryDetailsForm from "@/deliveries/components/PublicDeliveryDetailsForm";
import PublicDeliveryStepDetailsForm from "@/steps/components/PublicDeliveryStepDetailsForm";
import PublicDeliveryPrice from "@/deliveries/components/PublicDeliveryPrice";
import usePublicDeliveryFormViewModel from "@/deliveries/viewModel/usePublicDeliveryFormViewModel";
import React from "react";
import {useController} from "react-hook-form";

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
        <>
            <SegmentedButtons
                value={field.value.toString()}
                onValueChange={(value) => field.onChange(parseInt(value))}
                buttons={viewModel.deliveryTypes.map(b => ({
                    ...b,
                    style: {width: 100}
                }))}
            />
            <PublicDeliveryStepForm
                control={viewModel.control}
                totalDistance={0}
                canAddStep={field.value.toString() === viewModel.deliveryTypes[0].value}
            />
            <PublicDeliveryDetailsForm
                control={viewModel.control}
                errors={viewModel.errors}
                urgencies={viewModel.urgencies}
                packingSizes={viewModel.packingSizes}
            />
            <PublicDeliveryPrice
                price={0}
                priceWithTaxes={0}
            />
            <PublicDeliveryStepDetailsForm
                control={viewModel.control}
                errors={viewModel.errors}
                customerType={viewModel.customerType}
                setCustomerType={viewModel.setCustomerType}
            />
        </>
    );
}