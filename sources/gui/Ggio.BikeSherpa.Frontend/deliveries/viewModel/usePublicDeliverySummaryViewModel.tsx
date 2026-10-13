import {publicDeliveryStore$} from "@/deliveries/store/publicDeliveryStore";
import {IOCContainer} from "@/bootstrapper/constants/IOCContainer";
import IPublicDeliveryService from "@/deliveries/spi/IPublicDeliveryService";
import {DeliveryServiceIdentifier} from "@/deliveries/bootstrapper/DeliveryServiceIdentifier";
import PublicDeliverySummaryViewModel from "@/deliveries/viewModel/PublicDeliverySummaryViewModel";
import {useForm} from "react-hook-form";
import {PublicDeliveryCustomerFormValues} from "@/deliveries/models/zod/publicDeliveryCustomerFormBaseSchema";
import {useMemo, useState} from "react";
import {ILogger} from "@/spi/LogsSPI";
import {ServicesIdentifiers} from "@/bootstrapper/constants/ServicesIdentifiers";
import usePublicDeliveryModal from "@/deliveries/hooks/usePublicDeliveryModal";
import useDropdown from "@/hooks/useDropdown";
import {PublicDeliveryCustomerTypeEnum} from "@/deliveries/data/PublicDeliveryCustomerType";

export default function usePublicDeliverySummaryViewModel() {
    const publicDeliveryService = IOCContainer.get<IPublicDeliveryService>(DeliveryServiceIdentifier.PublicServices);
    const logger = IOCContainer.get<ILogger>(ServicesIdentifiers.Logger).extend("PublicDeliverySummary");
    const viewModel = new PublicDeliverySummaryViewModel(publicDeliveryService);

    const delivery = publicDeliveryStore$.delivery.get();
    const customer = publicDeliveryStore$.customer.get();
    const estimatedValue = publicDeliveryStore$.estimatedValue.get();

    let {urgencies} = useDropdown();
    
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
            name: customer?.name ?? '',
            email: customer?.email ?? '',
            phoneNumber: '',
            address: customer?.address ?? emptyAddress,
            needEstimate: false
        }
    });

    const {
        control,
        handleSubmit,
        formState: {errors}
    } = form;

    return {
        delivery,
        customer,
        estimatedValue,
        urgencyLabel: urgencies.find(urgency => urgency.value === delivery?.urgency),
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