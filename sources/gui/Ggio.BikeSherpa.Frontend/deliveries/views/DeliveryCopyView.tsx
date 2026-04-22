import {useLocalSearchParams} from "expo-router";
import DeliveryForm from "@/deliveries/components/DeliveryForm";
import {DeliveryFormValues} from "@/deliveries/models/zod/deliveryFormBaseSchema";
import {useDeliveryCopyFormViewModel} from "@/deliveries/viewModel/useDeliveryCopyFormViewModel";

export default function DeliveryCopyView() {
    const {deliveryId} = useLocalSearchParams<{ deliveryId: string }>();

    const {
        control,
        errors,
        handleSubmit,
        urgencies,
        pricingStrategies,
        packingSizes
    } = useDeliveryCopyFormViewModel(deliveryId);

    return (
        <DeliveryForm<DeliveryFormValues>
            control={control}
            errors={errors}
            handleSubmit={handleSubmit}
            buttonName="Copier la course"
            urgencies={urgencies}
            pricingStrategies={pricingStrategies}
            packingSizes={packingSizes}
            update
        />
    );
}