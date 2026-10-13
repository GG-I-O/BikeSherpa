import {useForm, useWatch} from "react-hook-form";
import {zodResolver} from "@hookform/resolvers/zod";
import {
    publicDeliveryFormBaseSchema,
    PublicDeliveryFormValues
} from "@/deliveries/models/zod/publicDeliveryFormBaseSchema";
import PublicDeliveryCustomer from "@/deliveries/models/PublicDeliveryCustomer";
import PublicDeliveryFormViewModel from "@/deliveries/viewModel/PublicDeliveryFormViewModel";
import IPublicDeliveryService from "@/deliveries/spi/IPublicDeliveryService";
import {IOCContainer} from "@/bootstrapper/constants/IOCContainer";
import {DeliveryServiceIdentifier} from "@/deliveries/bootstrapper/DeliveryServiceIdentifier";
import {useEffect, useMemo, useRef, useState} from "react";
import {PublicDeliveryCustomerTypeEnum} from "@/deliveries/data/PublicDeliveryCustomerType";
import {navigate} from "expo-router/build/global-state/routing";
import {publicDeliveryStore$} from "@/deliveries/store/publicDeliveryStore";
import {useDebounce} from "@/hooks/useDebounce";
import {useSelector} from "@legendapp/state/react";
import usePublicDeliveryModal from "@/deliveries/hooks/usePublicDeliveryModal";
import useDropdown from "@/hooks/useDropdown";

export default function usePublicDeliveryFormViewModel(customer?: PublicDeliveryCustomer) {
    const publicDeliveryService = IOCContainer.get<IPublicDeliveryService>(DeliveryServiceIdentifier.PublicServices);
    const viewModel = new PublicDeliveryFormViewModel(publicDeliveryService);

    let {pricingStrategies, urgencies, packingSizes} = useDropdown();

    const [customerType, setCustomerType] = useState<PublicDeliveryCustomerTypeEnum>(PublicDeliveryCustomerTypeEnum.Sender);

    const {setIsLoadingModalVisible, setIsErrorModalVisible} = usePublicDeliveryModal();

    const emptyAddress = useMemo(() => ({
        name: '',
        fullAddress: '',
        streetInfo: '',
        complement: '',
        postcode: '',
        city: '',
        coordinates: {longitude: 0, latitude: 0}
    }), []);

    const form = useForm<PublicDeliveryFormValues>({
        defaultValues: {
            pricingStrategy: customer?.deliveryType ?? 0,
            urgency: urgencies.length > 0 ? urgencies[0].value : 'Standard',
            totalPrice: 0,
            customer: {
                code: customer?.code ?? "xxx",
                name: customer?.name ?? '',
                email: customer?.email ?? '',
                phoneNumber: customer ? null : '',
                address: {
                    ...emptyAddress,
                    name: customer?.name ?? ''
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
                    stepAddress: emptyAddress,
                    packingSize: packingSizes.length > 0 ? packingSizes[0].value : 'S',
                },
                {
                    stepType: 1,
                    comment: '',
                    courierComment: '',
                    notBilled: false,
                    contactName: '',
                    contactPhone: '',
                    stepAddress: emptyAddress,
                    packingSize: packingSizes.length > 0 ? packingSizes[0].value : 'S',
                },
            ],
            insulatedBox: false,
            startDate: new Date().toISOString(),
            needEstimate: false,
        },
        resolver: zodResolver(publicDeliveryFormBaseSchema)
    });

    const {
        control,
        handleSubmit,
        formState: {errors},
        setValue,
        getValues
    } = form;

    // Hydrate infos depending on customer type
    useEffect(() => {
        const senderAddress = getValues("steps.0.stepAddress");
        const customerName = getValues("customer.name");
        const customerPhone = getValues("customer.phoneNumber") ?? "";

        setValue("customer.address", senderAddress);

        setValue("steps.0.stepAddress.name", customerName);
        setValue("steps.0.contactName", "");
        setValue("steps.0.contactPhone", customerPhone);

        setValue("steps.1.stepAddress.name", "");
        setValue("steps.1.contactName", "");
        setValue("steps.1.contactPhone", "");
    }, [emptyAddress, getValues, setValue]);

    // Keep Address fields populated when an address change
    useEffect(() => {
        const subscription = form.watch((values, {name, type}) => {
            const match = name?.match(/^steps\.(\d+)\.stepAddress$/);
            if (!match || type !== "change") return;

            const index = Number(match[1]);
            const address = values.steps?.[index]?.stepAddress;
            if (!address?.name) return;

            setValue(`steps.${index}.stepAddress.name`, address.name);

            if (!getValues(`steps.${index}.contactName`))
                setValue(`steps.${index}.contactName`, address.name);
        });

        return () => subscription.unsubscribe();
    }, [form, setValue, getValues]);

    // Multiple useWatch to keep dynamically updated field
    const stepAddresses = useWatch({control, name: "steps"});
    const stepCoordinates = useWatch({
        control,
        name: stepAddresses.map((_, index) => `steps.${index}.stepAddress.coordinates` as const)
    });
    const stepPackings = useWatch({
        control,
        name: stepAddresses.map((_, index) => `steps.${index}.packingSize` as const)
    });
    const triggerFields = useWatch({
        control,
        name: ["pricingStrategy", "urgency", "startDate"]
    });
    const cancelledRef = useRef(false);
    
    // Trigger estimated price calculation with debounce
    const estimateKey = useMemo(() => JSON.stringify({
        coords: stepCoordinates.map(c => [c?.longitude, c?.latitude]),
        packings: stepPackings,
        trigger: triggerFields
    }), [stepCoordinates, stepPackings, triggerFields]);
    
    const lastKeyRef = useRef<string | null>(null);

    useDebounce(() => {
        cancelledRef.current = false;

        const hasValidSteps = stepCoordinates.length >= 2
            && stepCoordinates.every(coord =>
                coord !== undefined && coord.longitude !== 0 && coord.latitude !== 0
            );
        if (!hasValidSteps) return;
        
        if (lastKeyRef.current === estimateKey) return;
        lastKeyRef.current = estimateKey;

        setIsLoadingModalVisible(true);

        viewModel.getEstimatedValue(getValues())
            .then((result) => {
                if (!cancelledRef.current) {
                    publicDeliveryStore$.estimatedValue.set(result);
                }
            })
            .catch(() => {
                lastKeyRef.current = null;
            })
            .finally(() => setIsLoadingModalVisible(false));

        return () => {
            cancelledRef.current = true;
        };
    }, 400, [estimateKey]);

    // Keep estimated price and distance dynamical
    const estimatedDistance = useSelector(() => publicDeliveryStore$.estimatedValue.get()?.totalDistance ?? 0);
    const estimatedPrice = useSelector(() => publicDeliveryStore$.estimatedValue.get()?.price ?? 0);
    const estimatedPriceWithTaxes = useSelector(() => publicDeliveryStore$.estimatedValue.get()?.priceWithVat ?? 0);

    // Slice first packingSize option if deliveryType === 1
    const deliveryTypes = pricingStrategies.slice(0, 2);
    const deliveryType = useWatch({control, name: "pricingStrategy"});
    const isLimited = deliveryTypes.length > 0 && deliveryType.toString() === deliveryTypes[1].value;
    useEffect(() => {
        if (!isLimited) return;
        stepPackings.forEach((packing, index) => {
            if (!packingSizes.some(ps => ps.value === packing))
                setValue(`steps.${index}.packingSize`, packingSizes[0].value);
        });
    }, [isLimited, packingSizes, stepPackings, setValue]);

    return {
        control,
        handleSubmit: handleSubmit(
            (data) => {
                setIsLoadingModalVisible(true);
                viewModel.onSubmit(data)
                    .then((isOk: boolean) => {
                        if (isOk)
                            navigate("/newDelivery/summary");
                        else
                            setIsErrorModalVisible(true);
                    })
                    .finally(() => setIsLoadingModalVisible(false));
            },
            (errors) => {
                console.error("Invalid delivery for creation");
                console.error(errors);
            }
        ),
        errors,
        deliveryTypes,
        packingSizes,
        customerType,
        setCustomerType,
        estimatedDistance: estimatedDistance,
        estimatedPrice: estimatedPrice,
        estimatedPriceWithTaxes: estimatedPriceWithTaxes,
        setUrgency: (urgency: string) => setValue("urgency", urgency),
        setStartDate: (startDate: string) => setValue("startDate", startDate)
    };
}