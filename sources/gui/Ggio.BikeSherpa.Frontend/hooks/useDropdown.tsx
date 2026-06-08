import {useEffect, useState} from "react";
import {DropdownOptions} from "@/models/DropdownOptions";
import {IOCContainer} from "@/bootstrapper/constants/IOCContainer";
import {IDropdownOptionsService} from "@/spi/IDropdownOptionsService";
import {ServicesIdentifiers} from "@/bootstrapper/constants/ServicesIdentifiers";

export default function useDropdown() {
    const dropdownService = IOCContainer.get<IDropdownOptionsService>(ServicesIdentifiers.DropdownOptionsService);
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