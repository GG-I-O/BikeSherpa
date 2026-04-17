import {IOCContainer} from "@/bootstrapper/constants/IOCContainer";
import {IDeliveryServices} from "../spi/IDeliveryServices";
import {DeliveryServiceIdentifier} from "../bootstrapper/DeliveryServiceIdentifier";
import {useForm} from "react-hook-form";
import {zodResolver} from "@hookform/resolvers/zod";
import {ICustomerService} from "@/spi/CustomerSPI";
import {ServicesIdentifiers} from "@/bootstrapper/constants/ServicesIdentifiers";
import {useEffect, useState} from "react";
import {DropdownOptions} from "@/models/DropdownOptions";
import {IDropdownOptions} from "@/spi/IDropdownOptions";
import Delivery from "@/deliveries/models/Delivery";
import {DeliveryFormValues} from "@/deliveries/models/zod/deliveryFormBaseSchema";
import useDeliveryDropdown from "@/deliveries/hooks/useDeliveryDropdown";
import DeliveryEditFormViewModel from "@/deliveries/viewModel/DeliveryEditFormViewModel";

export function useDeliveryEditFormViewModel(deliveryId: string) {
    const deliveryServices = IOCContainer.get<IDeliveryServices>(DeliveryServiceIdentifier.Services);
    const customerServices = IOCContainer.get<ICustomerService>(ServicesIdentifiers.CustomerServices);
    const viewModel = new DeliveryEditFormViewModel(deliveryServices, customerServices);
    
    const { urgencies, pricingStrategies, packingSizes } = useDeliveryDropdown();
    
    const delivery: Delivery = deliveryServices.getDelivery$(deliveryId).get();

    const {
        control,
        handleSubmit,
        formState: {errors}
    } = useForm<DeliveryFormValues>({
        defaultValues: {
            code: delivery.code,
            status: delivery.status,
            customerId: customerServices.getCustomer$(delivery.customerId).get().code,
            pricingStrategy: delivery.pricingStrategy,
            urgency: delivery.urgency,
            totalPrice: delivery.totalPrice ?? 0,
            discount: delivery.discount ?? 0,
            reportId: delivery.reportId ?? '',
            steps: delivery.steps,
            details: delivery.details,
            packingSize: delivery.packingSize,
            insulatedBox: delivery.insulatedBox,
            startDate: delivery.startDate,
            contractDate: delivery.contractDate,
        },
        resolver: zodResolver(viewModel.getEditDeliverySchema())
    });

    return {
        control,
        handleSubmit: handleSubmit(
            (data) => {
                viewModel.onSubmit(data, delivery);
            },
            (errors) => {
                console.error("Invalid delivery for creation");
                console.error(errors);
            }
        ),
        errors,
        urgencies,
        pricingStrategies,
        packingSizes
    };
}