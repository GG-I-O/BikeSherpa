import {useLocalSearchParams} from "expo-router";
import DeliveryForm from "@/deliveries/components/DeliveryForm";
import {DeliveryFormValues} from "@/deliveries/models/zod/deliveryFormBaseSchema";
import {useDeliveryCopyFormViewModel} from "@/deliveries/viewModel/useDeliveryCopyFormViewModel";
import {FormProvider} from "react-hook-form";

export default function DeliveryCopyView() {
    const {deliveryId} = useLocalSearchParams<{ deliveryId: string }>();

    const {
        form,
        errors,
        handleSubmit,
        getCustomerOptions,
        getCustomer,
        urgencies,
        pricingStrategies
    } = useDeliveryCopyFormViewModel(deliveryId);

    return (
        <FormProvider {...form} >
            <DeliveryForm<DeliveryFormValues>
                control={form.control}
                errors={errors}
                handleSubmit={handleSubmit}
                buttonName="Copier la course"
                getCustomerOptions={getCustomerOptions}
                getCustomer={getCustomer}
                urgencies={urgencies}
                pricingStrategies={pricingStrategies}
                update
            />
        </FormProvider>
    );
}