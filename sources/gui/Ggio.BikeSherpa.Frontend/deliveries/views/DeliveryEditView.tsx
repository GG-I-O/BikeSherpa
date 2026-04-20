import DeliveryForm from "@/deliveries/components/DeliveryForm";
import {DeliveryFormValues} from "@/deliveries/models/zod/deliveryFormBaseSchema";
import {useDeliveryEditFormViewModel} from "@/deliveries/viewModel/useDeliveryEditFormViewModel";
import {useLocalSearchParams} from "expo-router";

export default function DeliveryEditView() {
    const {deliveryId} = useLocalSearchParams<{ deliveryId: string }>();
    
    const {
        control,
        errors,
        handleSubmit,
        urgencies,
        pricingStrategies,
        packingSizes
    } = useDeliveryEditFormViewModel(deliveryId);

    return (
        <DeliveryForm<DeliveryFormValues>
            control={control}
            errors={errors}
            handleSubmit={handleSubmit}
            buttonName="Mettre à jour la course"
            urgencies={urgencies}
            pricingStrategies={pricingStrategies}
            packingSizes={packingSizes}
            update
        />
    );
}