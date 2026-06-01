import {useForm} from "react-hook-form";
import {deliveryCustomerSchema, DeliveryCustomerValues} from "@/deliveries/models/zod/deliveryCustomerSchema";
import {zodResolver} from "@hookform/resolvers/zod";
import {useEffect, useState} from "react";
import PublicDeliveryCustomer from "@/deliveries/models/PublicDeliveryCustomer";
import IPublicDeliveryService from "@/deliveries/spi/IPublicDeliveryService";
import {IOCContainer} from "@/bootstrapper/constants/IOCContainer";
import {DeliveryServiceIdentifier} from "@/deliveries/bootstrapper/DeliveryServiceIdentifier";
import PublicDeliveryLoginFormViewModel from "@/deliveries/viewModel/PublicDeliveryLoginFormViewModel";

export default function usePublicDeliveryLoginFormViewModel(
    login: (customer?: PublicDeliveryCustomer) => void
) {
    const publicDeliveryService = IOCContainer.get<IPublicDeliveryService>(DeliveryServiceIdentifier.PublicServices);
    const viewModel = new PublicDeliveryLoginFormViewModel(publicDeliveryService);

    const [publicCustomer, setPublicCustomer] = useState<PublicDeliveryCustomer | null>(null);

    useEffect(() => {
        if (publicCustomer && publicCustomer.email && publicCustomer.code)
            login(publicCustomer);
    }, [publicCustomer])

    const {
        control,
        handleSubmit,
        formState: {errors},
        setError,
        clearErrors
    } = useForm<DeliveryCustomerValues>({
        defaultValues: {
            email: '',
            code: ''
        },
        resolver: zodResolver(deliveryCustomerSchema)
    });

    return {
        control,
        handleSubmit: handleSubmit(
            (data) => {
                viewModel.login(data.email, data.code)
                    .then(customer => {
                            if (!customer) {
                                setError("email",
                                    {
                                        type: "manual",
                                        message: "Email et/ou code invalide"
                                    });
                                setPublicCustomer(null);
                            } else {
                                clearErrors("email");
                                setPublicCustomer({
                                    email: data.email,
                                    code: data.code,
                                    name: customer.name,
                                    deliveryType: customer.deliveryType
                                });
                            }
                        }
                    )
            },
            (errors) => {
                console.error("Invalid delivery for creation");
                console.error(errors);
            }
        ),
        errors
    }
}