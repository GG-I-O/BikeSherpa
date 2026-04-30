import {useNewDeliveryFormViewModel} from "@/deliveries/viewModel/useNewDeliveryFormViewModel";
import DeliveryForm from "@/deliveries/components/DeliveryForm";
import {DeliveryFormValues} from "@/deliveries/models/zod/deliveryFormBaseSchema";

export default function NewDeliveryView() {
    const {control,
        errors,
        handleSubmit,
        getCustomerOptions,
        urgencies,
        pricingStrategies,
        packingSizes
    } = useNewDeliveryFormViewModel();

    return (
        <DeliveryForm<DeliveryFormValues>
            control={control}
            errors={errors}
            handleSubmit={handleSubmit}
            buttonName="Ajouter la course"
            getCustomerOptions={getCustomerOptions}
            urgencies={urgencies}
            pricingStrategies={pricingStrategies}
            packingSizes={packingSizes}
        />
    );
}