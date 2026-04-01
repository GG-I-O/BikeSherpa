import { IOCContainer } from "@/bootstrapper/constants/IOCContainer";
import { IDeliveryServices } from "../spi/IDeliveryServices";
import { DeliveryServiceIdentifier } from "../bootstrapper/DeliveryServiceIdentifier";
import NewDeliveryFormViewModel from "./NewDeliveryFormViewModel";
import { useForm } from "react-hook-form";
import InputDelivery from "../models/InputDelivery";
import { DeliveryPacking } from "../models/DeliveryPacking";
import { zodResolver } from "@hookform/resolvers/zod";

export function useNewDeliveryFormViewModel() {
    const deliveryServices = IOCContainer.get<IDeliveryServices>(DeliveryServiceIdentifier.Services);
    const newDeliveryViewModel = new NewDeliveryFormViewModel(deliveryServices);
    
    const {
        control,
        handleSubmit,
        formState: {errors},
        reset
    } = useForm<InputDelivery>({
        defaultValues: {
            code: '',
            status: 0,
            customerId: '',
            pricingStrategy: 0,
            urgency: '',
            totalPrice: 0,
            discount: 0,
            reportId: '',
            details: [],
            packingSize: DeliveryPacking.S.toString(),
            insulatedBox: false,
        },
        resolver: zodResolver(newDeliveryViewModel.getNewDeliverySchema())
    });

    newDeliveryViewModel.setResetCallback(reset);

    return {
        control,
        handleSubmit: handleSubmit(newDeliveryViewModel.onSubmit),
        errors
    };
}