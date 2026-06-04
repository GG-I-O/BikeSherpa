import {useForm} from "react-hook-form";
import {deliveryCustomerSchema, DeliveryCustomerValues} from "@/deliveries/models/zod/deliveryCustomerSchema";
import {zodResolver} from "@hookform/resolvers/zod";
import {useEffect, useState} from "react";
import PublicDeliveryCustomer from "@/deliveries/models/PublicDeliveryCustomer";
import IPublicDeliveryService from "@/deliveries/spi/IPublicDeliveryService";
import {IOCContainer} from "@/bootstrapper/constants/IOCContainer";
import {DeliveryServiceIdentifier} from "@/deliveries/bootstrapper/DeliveryServiceIdentifier";
import PublicDeliveryLoginViewModel from "@/deliveries/viewModel/PublicDeliveryLoginViewModel";
import {useRouter} from "expo-router";
import {publicDeliveryStore$} from "@/deliveries/store/publicDeliveryStore";
import {navigate} from "expo-router/build/global-state/routing";

export default function usePublicDeliveryLoginViewModel() {
    const publicDeliveryService = IOCContainer.get<IPublicDeliveryService>(DeliveryServiceIdentifier.PublicServices);
    const viewModel = new PublicDeliveryLoginViewModel(publicDeliveryService);
    
    const router = useRouter();

    const [publicCustomer, setPublicCustomer] = useState<PublicDeliveryCustomer | null>(null);
    const [isLoading, setIsLoading] = useState<boolean>(false);

    useEffect(() => {
        if (publicCustomer && publicCustomer.email && publicCustomer.code) {
            setIsLoading(false);
            publicDeliveryStore$.customer.set(publicCustomer);
            publicDeliveryStore$.isAnonymous.set(false);
            navigate("/newDelivery/form");
        }
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
                setIsLoading(true);
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
                    .finally(() => setIsLoading(false))
            },
            (errors) => {
                console.error("Invalid delivery for creation");
                console.error(errors);
            }
        ),
        errors,
        proceedAsAnonymous: () => {
            publicDeliveryStore$.customer.set(null);
            publicDeliveryStore$.isAnonymous.set(true);
            router.push("/newDelivery/form");
        },
        isLoading
    }
}