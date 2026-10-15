import {publicDeliveryStore$} from "@/deliveries/store/publicDeliveryStore";
import {IOCContainer} from "@/bootstrapper/constants/IOCContainer";
import IPublicDeliveryService from "@/deliveries/spi/IPublicDeliveryService";
import {DeliveryServiceIdentifier} from "@/deliveries/bootstrapper/DeliveryServiceIdentifier";
import PublicDeliverySummaryViewModel from "@/deliveries/viewModel/PublicDeliverySummaryViewModel";
import {useForm} from "react-hook-form";
import {
    publicDeliveryCustomerFormBaseSchema,
    PublicDeliveryCustomerFormValues
} from "@/deliveries/models/zod/publicDeliveryCustomerFormBaseSchema";
import {useEffect, useMemo, useState} from "react";
import {ILogger} from "@/spi/LogsSPI";
import {ServicesIdentifiers} from "@/bootstrapper/constants/ServicesIdentifiers";
import usePublicDeliveryModal from "@/deliveries/hooks/usePublicDeliveryModal";
import useDropdown from "@/hooks/useDropdown";
import {PublicDeliveryCustomerTypeEnum} from "@/deliveries/data/PublicDeliveryCustomerType";
import {zodResolver} from "@hookform/resolvers/zod";

export default function usePublicDeliverySummaryViewModel() {
    const publicDeliveryService = IOCContainer.get<IPublicDeliveryService>(DeliveryServiceIdentifier.PublicServices);
    const logger = IOCContainer.get<ILogger>(ServicesIdentifiers.Logger).extend("PublicDeliverySummary");
    const viewModel = new PublicDeliverySummaryViewModel(publicDeliveryService);

    const delivery = publicDeliveryStore$.delivery.get();
    const customer = publicDeliveryStore$.customer.get();
    const estimatedValue = publicDeliveryStore$.estimatedValue.get();

    let {urgencies, packingSizes} = useDropdown();
    
    const {setIsLoadingModalVisible, setIsErrorModalVisible} = usePublicDeliveryModal();
    
    const [customerType, setCustomerType] = useState<PublicDeliveryCustomerTypeEnum>(PublicDeliveryCustomerTypeEnum.Sender);

    const emptyAddress = useMemo(() => ({
        name: '',
        fullAddress: '',
        streetInfo: '',
        complement: '',
        postcode: '',
        city: '',
        coordinates: {longitude: 0, latitude: 0}
    }), []);

    const form = useForm<PublicDeliveryCustomerFormValues>({
        defaultValues: {
            code: customer?.code ?? '',
            name: customer?.name ?? (customer?.address.name ?? ''),
            email: customer?.email ?? '',
            phoneNumber: '',
            address: customer?.address ? {...customer.address, name: ''} : emptyAddress,
            needEstimate: false
        },
        resolver: zodResolver(publicDeliveryCustomerFormBaseSchema),
        shouldFocusError: false
    });

    const {
        control,
        handleSubmit,
        formState: {errors},
        setValue
    } = form;
    
    useEffect(() => {
        if (customer || !delivery)
            return;
        switch(customerType) {
            case PublicDeliveryCustomerTypeEnum.Sender:
                setValue("address", delivery.steps[0].stepAddress);
                setValue("address.name", delivery.steps[0].stepAddress.name);
                setValue("phoneNumber", delivery.steps[0].stepAddress.phone);
                break;
            case PublicDeliveryCustomerTypeEnum.Receiver:
                setValue("address", delivery.steps[1].stepAddress);
                setValue("address.name", delivery.steps[1].stepAddress.name);
                setValue("phoneNumber", delivery.steps[1].stepAddress.phone);
                break;
            case PublicDeliveryCustomerTypeEnum.None:
                setValue("address", emptyAddress);
                setValue("address.name", '');
                setValue("phoneNumber", '');
                break;
        }
    },[customer, delivery, customerType, emptyAddress, setValue])

    return {
        delivery,
        customer,
        estimatedValue,
        urgencyLabel: urgencies.find(urgency => urgency.value === delivery?.urgency),
        getPackingSizeLabel: (packingSize: string) => packingSizes.find(packing => packing.value === packingSize),
        control,
        handleSubmit: handleSubmit(
            (data) => {
                setIsLoadingModalVisible(true);
                const deliveryObject = {...delivery!, needEstimate: data.needEstimate};
                const customerObject = viewModel.mapFormValuesToObject(data);
                viewModel.createDelivery(deliveryObject, customerObject)
                    .then((isOk: boolean) =>
                        isOk ? {} : setIsErrorModalVisible(true)
                    )
                    .finally(() => setIsLoadingModalVisible(false));
            },
            (errors) => {
                logger.error("Invalid delivery for creation");
                logger.error(errors);
            }
        ),
        errors,
        customerType,
        setCustomerType
    }
}