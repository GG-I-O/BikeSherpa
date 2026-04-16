import {IOCContainer} from "@/bootstrapper/constants/IOCContainer";
import {IDeliveryServices} from "../spi/IDeliveryServices";
import {DeliveryServiceIdentifier} from "../bootstrapper/DeliveryServiceIdentifier";
import NewDeliveryFormViewModel from "./NewDeliveryFormViewModel";
import {useForm} from "react-hook-form";
import {zodResolver} from "@hookform/resolvers/zod";
import {ICustomerService} from "@/spi/CustomerSPI";
import {ServicesIdentifiers} from "@/bootstrapper/constants/ServicesIdentifiers";
import {useEffect, useState} from "react";
import {DropdownOptions} from "@/models/DropdownOptions";
import {IDropdownOptions} from "@/spi/IDropdownOptions";
import Delivery from "@/deliveries/models/Delivery";
import {DeliveryFormValues} from "@/deliveries/models/zod/deliveryFormBaseSchema";

export function useNewDeliveryFormViewModel() {
    const deliveryServices = IOCContainer.get<IDeliveryServices>(DeliveryServiceIdentifier.Services);
    const customerServices = IOCContainer.get<ICustomerService>(ServicesIdentifiers.CustomerServices);
    const newDeliveryViewModel = new NewDeliveryFormViewModel(deliveryServices, customerServices);

    const dropdownService = IOCContainer.get<IDropdownOptions<Delivery>>(DeliveryServiceIdentifier.DropdownOptionsService);

    const [urgencies, setUrgencies] = useState<DropdownOptions[]>([]);
    const [pricingStrategies, setPricingStrategies] = useState<DropdownOptions[]>([]);
    const [packingSizes, setPackingSizes] = useState<DropdownOptions[]>([]);

    useEffect(() => {
        async function getOptions() {
            const options = await dropdownService.GetOptions();
            setUrgencies(options["urgencies"]);
            setPricingStrategies(options["pricingStrategies"]);
            setPackingSizes(options["packingsSizes"]);
        }

        getOptions().then();
    }, [dropdownService]);

    const {
        control,
        handleSubmit,
        formState: {errors},
        reset
    } = useForm<DeliveryFormValues>({
        defaultValues: {
            code: '',
            status: 0,
            customerId: '',
            pricingStrategy: pricingStrategies.length > 0 ? parseInt(packingSizes[0].value) : 1,
            urgency: urgencies.length > 0 ? urgencies[0].value : 'Standard',
            totalPrice: 0,
            discount: 0,
            reportId: '',
            steps: [],
            details: [""],
            packingSize: packingSizes.length > 0 ? packingSizes[0].value : 'L',
            insulatedBox: false,
            startDate: new Date().toISOString(),
            contractDate: new Date().toISOString(),
        },
        resolver: zodResolver(newDeliveryViewModel.getNewDeliverySchema())
    });

    newDeliveryViewModel.setResetCallback(reset);

    return {
        control,
        handleSubmit: handleSubmit(
            (data) => {
                newDeliveryViewModel.onSubmit(data);
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