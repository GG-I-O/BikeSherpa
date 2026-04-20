import Delivery from "@/deliveries/models/Delivery";
import {useForm} from "react-hook-form";
import {DeliveryFormValues} from "@/deliveries/models/zod/deliveryFormBaseSchema";
import {useEffect, useState} from "react";
import {DropdownOptions} from "@/models/DropdownOptions";
import {IOCContainer} from "@/bootstrapper/constants/IOCContainer";
import {IDropdownOptions} from "@/spi/IDropdownOptions";
import {DeliveryServiceIdentifier} from "@/deliveries/bootstrapper/DeliveryServiceIdentifier";

export default function useDeliveryDropdown() {
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
    
    return {
        urgencies,
        pricingStrategies,
        packingSizes,
    }
}