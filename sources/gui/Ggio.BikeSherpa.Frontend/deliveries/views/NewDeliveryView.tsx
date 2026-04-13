import {useNewDeliveryFormViewModel} from "@/deliveries/viewModel/useNewDeliveryFormViewModel";
import InputDelivery from "@/deliveries/models/InputDelivery";
import DeliveryForm from "@/deliveries/components/DeliveryForm";

export default function NewDeliveryView() {
    const {control, errors, handleSubmit, urgencies, pricingStrategies, packingSizes} = useNewDeliveryFormViewModel();

    return (
        <DeliveryForm<InputDelivery>
            control={control}
            errors={errors}
            handleSubmit={handleSubmit}
            buttonName="Ajouter la course"
            urgencies={urgencies}
            pricingStrategies={pricingStrategies}
            packingSizes={packingSizes}
        />
    );
}