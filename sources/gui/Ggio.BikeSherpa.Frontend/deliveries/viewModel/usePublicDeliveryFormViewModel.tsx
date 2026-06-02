import {useForm} from "react-hook-form";
import {zodResolver} from "@hookform/resolvers/zod";
import {
    publicDeliveryFormBaseSchema,
    PublicDeliveryFormValues
} from "@/deliveries/models/zod/publicDeliveryFormBaseSchema";
import PublicDeliveryCustomer from "@/deliveries/models/PublicDeliveryCustomer";
import useDeliveryDropdown from "@/deliveries/hooks/useDeliveryDropdown";
import PublicDeliveryFormViewModel from "@/deliveries/viewModel/PublicDeliveryFormViewModel";
import IPublicDeliveryService from "@/deliveries/spi/IPublicDeliveryService";
import {IOCContainer} from "@/bootstrapper/constants/IOCContainer";
import {DeliveryServiceIdentifier} from "@/deliveries/bootstrapper/DeliveryServiceIdentifier";
import {useState} from "react";
import {PublicDeliveryCustomerTypeEnum} from "@/deliveries/data/PublicDeliveryCustomerType";

export default function usePublicDeliveryFormViewModel(customer?: PublicDeliveryCustomer) {
    const publicDeliveryService = IOCContainer.get<IPublicDeliveryService>(DeliveryServiceIdentifier.PublicServices);
    const viewModel = new PublicDeliveryFormViewModel(publicDeliveryService, customer);

    const {urgencies, pricingStrategies, packingSizes} = useDeliveryDropdown();
    
    const [customerType, setCustomerType] = useState<PublicDeliveryCustomerTypeEnum>(PublicDeliveryCustomerTypeEnum.None);

    const {
        control,
        handleSubmit,
        formState: {errors},
    } = useForm<PublicDeliveryFormValues>({
        defaultValues: {
            pricingStrategy: customer?.deliveryType ?? 1,
            urgency: urgencies.length > 0 ? urgencies[0].value : 'Standard',
            totalPrice: 0,
            customer: {
                name: customer?.name ?? '',
                email: customer?.email ?? '',
                phoneNumber: customer ? null : '06',
                address: {
                    name: '',
                    streetInfo: '',
                    complement: '',
                    postcode: '',
                    city: '',
                }
            },
            steps: [],
            packingSize: packingSizes.length > 0 ? packingSizes[0].value : 'S',
            insulatedBox: false,
            startDate: new Date().toISOString(),
        },
        resolver: zodResolver(publicDeliveryFormBaseSchema)
    });

    return {
        control,
        handleSubmit: handleSubmit(
            (data) => {
                viewModel.onSubmit(data);
            },
            (errors) => {
                console.error("Invalid delivery for creation");
                console.error(errors);
            }
        ),
        errors,
        urgencies,
        deliveryTypes: pricingStrategies.slice(1),
        packingSizes,
        customerType,
        setCustomerType
    };
}