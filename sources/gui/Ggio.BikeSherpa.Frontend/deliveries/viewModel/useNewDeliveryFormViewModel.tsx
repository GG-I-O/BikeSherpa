import { IOCContainer } from "@/bootstrapper/constants/IOCContainer";
import { IDeliveryServices } from "../spi/IDeliveryServices";
import { DeliveryServiceIdentifier } from "../bootstrapper/DeliveryServiceIdentifier";
import NewDeliveryFormViewModel from "./NewDeliveryFormViewModel";
import { useForm } from "react-hook-form";
import InputDelivery from "../models/InputDelivery";
import { zodResolver } from "@hookform/resolvers/zod";
import packingSize from "@/deliveries/models/dropdownOptions/packingSize";
import {ICustomerService} from "@/spi/CustomerSPI";
import {ServicesIdentifiers} from "@/bootstrapper/constants/ServicesIdentifiers";

export function useNewDeliveryFormViewModel() {
    const deliveryServices = IOCContainer.get<IDeliveryServices>(DeliveryServiceIdentifier.Services);
    const customerServices = IOCContainer.get<ICustomerService>(ServicesIdentifiers.CustomerServices);
    const newDeliveryViewModel = new NewDeliveryFormViewModel(deliveryServices, customerServices);
    
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
            pricingStrategy: 1,
            urgency: 'Standard',
            totalPrice: 0,
            discount: 0,
            reportId: '',
            details: [""],
            packingSize: packingSize[0].value,
            insulatedBox: false,
            startDate: new Date().toISOString(),
            contractDate: new Date().toISOString(),
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