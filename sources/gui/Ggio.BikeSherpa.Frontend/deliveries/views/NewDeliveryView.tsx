import {useNewDeliveryFormViewModel} from "@/deliveries/viewModel/useNewDeliveryFormViewModel";
import DeliveryForm from "@/deliveries/components/DeliveryForm";
import {DeliveryFormValues} from "@/deliveries/models/zod/deliveryFormBaseSchema";
import {FormProvider} from "react-hook-form";

export default function NewDeliveryView() {
    const {
        form,
        errors,
        handleSubmit,
        getCustomerOptions,
        urgencies,
        pricingStrategies
    } = useNewDeliveryFormViewModel();

    return (
        <FormProvider {...form} >
            <DeliveryForm<DeliveryFormValues>
                control={form.control}
                errors={errors}
                handleSubmit={handleSubmit}
                buttonName="Ajouter la course"
                getCustomerOptions={getCustomerOptions}
                urgencies={urgencies}
                pricingStrategies={pricingStrategies}
            />
        </FormProvider>
    );
}