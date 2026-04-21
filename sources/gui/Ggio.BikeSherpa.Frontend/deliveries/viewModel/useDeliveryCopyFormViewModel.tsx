import {IOCContainer} from "@/bootstrapper/constants/IOCContainer";
import {IDeliveryServices} from "@/deliveries/spi/IDeliveryServices";
import {DeliveryServiceIdentifier} from "@/deliveries/bootstrapper/DeliveryServiceIdentifier";
import {ICustomerService} from "@/spi/CustomerSPI";
import {ServicesIdentifiers} from "@/bootstrapper/constants/ServicesIdentifiers";
import DeliveryEditFormViewModel from "@/deliveries/viewModel/DeliveryEditFormViewModel";
import useDeliveryDropdown from "@/deliveries/hooks/useDeliveryDropdown";
import Delivery from "@/deliveries/models/Delivery";
import {useForm} from "react-hook-form";
import {DeliveryFormValues} from "@/deliveries/models/zod/deliveryFormBaseSchema";
import {zodResolver} from "@hookform/resolvers/zod";
import DeliveryCopyFormViewModel from "@/deliveries/viewModel/DeliveryCopyFormViewModel";
import {navigate} from "expo-router/build/global-state/routing";

export function useDeliveryCopyFormViewModel(deliveryId: string) {
    const deliveryServices = IOCContainer.get<IDeliveryServices>(DeliveryServiceIdentifier.Services);
    const customerServices = IOCContainer.get<ICustomerService>(ServicesIdentifiers.CustomerServices);
    const viewModel = new DeliveryCopyFormViewModel(deliveryServices, customerServices);

    const {urgencies, pricingStrategies, packingSizes} = useDeliveryDropdown();

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
        resolver: zodResolver(viewModel.getCopyDeliverySchema())
    });

    return {
        control,
        handleSubmit: handleSubmit(
            (data) => {
                viewModel.onSubmit(data);
                navigate({
                    pathname: '/(tabs)/(deliveries)',
                });
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