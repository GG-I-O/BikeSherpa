import {IOCContainer} from "@/bootstrapper/constants/IOCContainer";
import {IDeliveryServices} from "@/deliveries/spi/IDeliveryServices";
import {DeliveryServiceIdentifier} from "@/deliveries/bootstrapper/DeliveryServiceIdentifier";
import {ICustomerService} from "@/spi/CustomerSPI";
import {ServicesIdentifiers} from "@/bootstrapper/constants/ServicesIdentifiers";
import Delivery from "@/deliveries/models/Delivery";
import {useForm} from "react-hook-form";
import {DeliveryFormValues} from "@/deliveries/models/zod/deliveryFormBaseSchema";
import {zodResolver} from "@hookform/resolvers/zod";
import NewDeliveryFormViewModel from "@/deliveries/viewModel/NewDeliveryFormViewModel";
import useDropdown from "@/hooks/useDropdown";
import {router} from "expo-router";

export function useDeliveryCopyFormViewModel(deliveryId: string) {
    const deliveryServices = IOCContainer.get<IDeliveryServices>(DeliveryServiceIdentifier.Services);
    const customerServices = IOCContainer.get<ICustomerService>(ServicesIdentifiers.CustomerServices);
    const viewModel = new NewDeliveryFormViewModel(deliveryServices, customerServices);

    const {urgencies, pricingStrategies, packingSizes} = useDropdown();

    const delivery: Delivery = deliveryServices.getDelivery$(deliveryId).get();

    const {
        control,
        handleSubmit,
        formState: {errors}
    } = useForm<DeliveryFormValues>({
        defaultValues: {
            code: delivery.code,
            status: delivery.status,
            customerId: customerServices.getCustomer$(delivery.customerId).get().id,
            pricingStrategy: delivery.pricingStrategy,
            urgency: delivery.urgency,
            totalPrice: delivery.totalPrice ?? 0,
            discount: delivery.discount ?? 0,
            discountReason: delivery.discountReason ?? "",
            extraCost: delivery.extraCost ?? 0,
            extraCostReason: delivery.extraCostReason ?? "",
            steps: delivery.steps.map(step => ({
                ...step,
                contactName: step.stepAddress.name,
                contactPhone: step.stepAddress.phone ?? ''
            })),
            details: delivery.details,
            insulatedBox: delivery.insulatedBox,
            startDate: delivery.startDate,
            contractDate: delivery.contractDate,
        },
        resolver: zodResolver(viewModel.getDeliverySchema())
    });

    return {
        control,
        handleSubmit: handleSubmit(
            (data) => {
                viewModel.onSubmit(data);
                router.back();
            },
            (errors) => {
                console.error("Invalid delivery for creation");
                console.error(errors);
            }
        ),
        errors,
        urgencies,
        pricingStrategies,
        packingSizes,
        getCustomerOptions: viewModel.getCustomerOptions,
        getCustomer: (id: string) => customerServices.getCustomer$(id).peek(),
    };
}