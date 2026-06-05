import {useForm, useWatch} from "react-hook-form";
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
import {useEffect, useRef, useState} from "react";
import {PublicDeliveryCustomerTypeEnum} from "@/deliveries/data/PublicDeliveryCustomerType";
import {navigate} from "expo-router/build/global-state/routing";
import {publicDeliveryStore$} from "@/deliveries/store/publicDeliveryStore";
import {useDebounce} from "@/hooks/useDebounce";
import {useSelector} from "@legendapp/state/react";

export default function usePublicDeliveryFormViewModel(customer?: PublicDeliveryCustomer) {
    const publicDeliveryService = IOCContainer.get<IPublicDeliveryService>(DeliveryServiceIdentifier.PublicServices);
    const viewModel = new PublicDeliveryFormViewModel(publicDeliveryService);

    const {pricingStrategies, urgencies, packingSizes} = useDeliveryDropdown();

    const [customerType, setCustomerType] = useState<PublicDeliveryCustomerTypeEnum>(PublicDeliveryCustomerTypeEnum.Sender);
    const [isLoading, setIsLoading] = useState<boolean>(false);
    const [showErrorModal, setShowErrorModal] = useState<boolean>(false);

    const {
        control,
        handleSubmit,
        formState: {errors},
        setValue,
        getValues
    } = useForm<PublicDeliveryFormValues>({
        defaultValues: {
            pricingStrategy: customer?.deliveryType ?? 1,
            urgency: urgencies.length > 0 ? urgencies[0].value : 'Standard',
            totalPrice: 0,
            customer: {
                name: customer?.name ?? '',
                email: customer?.email ?? '',
                phoneNumber: customer ? null : '',
                address: {
                    name: customer?.name ?? '',
                    streetInfo: '',
                    complement: '',
                    postcode: '',
                    city: '',
                }
            },
            steps: [
                {
                    stepType: 0,
                    comment: '',
                    courierComment: '',
                    notBilled: false,
                    contactName: '',
                    contactPhone: '',
                    stepAddress: {
                        name: '',
                        fullAddress: '',
                        streetInfo: '',
                        complement: '',
                        postcode: '',
                        city: '',
                        coordinates: {longitude: 0, latitude: 0}
                    }
                },
                {
                    stepType: 1,
                    comment: '',
                    courierComment: '',
                    notBilled: false,
                    contactName: '',
                    contactPhone: '',
                    stepAddress: {
                        name: '',
                        fullAddress: '',
                        streetInfo: '',
                        complement: '',
                        postcode: '',
                        city: '',
                        coordinates: {longitude: 0, latitude: 0}
                    }
                },
            ],
            packingSize: packingSizes.length > 0 ? packingSizes[0].value : 'S',
            insulatedBox: false,
            startDate: new Date().toISOString(),
            needEstimate: false,
        },
        resolver: zodResolver(publicDeliveryFormBaseSchema)
    });

    const stepAddresses = useWatch({control, name: "steps"});

    useEffect(() => {
        if (customerType === PublicDeliveryCustomerTypeEnum.Sender) {
            const senderAddress = stepAddresses?.[0]?.stepAddress;
            if (senderAddress) {
                setValue("customer.address", senderAddress);
            }
        } else if (customerType === PublicDeliveryCustomerTypeEnum.Receiver) {
            const receiverAddress = stepAddresses?.[1]?.stepAddress;
            if (receiverAddress) {
                setValue("customer.address", receiverAddress);
            }
        }
    }, [customerType, stepAddresses, setValue]);

    const triggerFields = useWatch({
        control,
        name: ["steps", "pricingStrategy", "packingSize", "urgency", "startDate"]
    });

    const cancelledRef = useRef(false);

    useDebounce(() => {
        cancelledRef.current = false;

        const [steps] = triggerFields;
        if (!steps?.[0]?.stepAddress?.city || !steps?.[1]?.stepAddress?.city) return;
        
        setIsLoading(true);

        viewModel.getEstimatedValue(getValues(), customerType)
            .then((result) => {
                if (!cancelledRef.current) {
                    publicDeliveryStore$.estimatedValue.set(result);
                }
            })
            .finally(() => setIsLoading(false));

        return () => {
            cancelledRef.current = true;
        };
    }, 400, [triggerFields, customerType]);

    const estimatedDistance = useSelector(() => publicDeliveryStore$.estimatedValue.get()?.totalDistance ?? 0);
    const estimatedPrice = useSelector(() => publicDeliveryStore$.estimatedValue.get()?.price ?? 0);
    const estimatedPriceWithTaxes = useSelector(() => publicDeliveryStore$.estimatedValue.get()?.priceWithVat ?? 0);

    return {
        control,
        handleSubmit: handleSubmit(
            (data) => {
                setIsLoading(true);
                viewModel.onSubmit(data, customerType)
                    .then((isOk: boolean) => {
                        if (isOk)
                            navigate("/newDelivery/summary");
                        else
                            setShowErrorModal(true);
                    })
                    .finally(() => setIsLoading(false));
            },
            (errors) => {
                console.error("Invalid delivery for creation");
                console.error(errors);
            }
        ),
        errors,
        deliveryTypes: pricingStrategies.slice(0, 2),
        packingSizes,
        customerType,
        setCustomerType,
        isLoading,
        showErrorModal,
        setShowErrorModal,
        goToLogin: () => navigate("/newDelivery"),
        estimatedDistance: estimatedDistance,
        estimatedPrice: estimatedPrice,
        estimatedPriceWithTaxes: estimatedPriceWithTaxes,
        setUrgency: (urgency: string) => setValue("urgency", urgency),
        setStartDate: (startDate: string) => setValue("startDate", startDate)
    };
}