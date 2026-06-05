import {IOCContainer} from "@/bootstrapper/constants/IOCContainer";
import IPublicDeliveryService from "@/deliveries/spi/IPublicDeliveryService";
import {DeliveryServiceIdentifier} from "@/deliveries/bootstrapper/DeliveryServiceIdentifier";
import {PublicDeliveryPriceViewModel} from "@/deliveries/viewModel/PublicDeliveryPriceViewModel";
import {useEffect, useMemo, useState} from "react";

export default function usePublicDeliveryPriceViewModel() {
    const publicDeliveryService = IOCContainer.get<IPublicDeliveryService>(DeliveryServiceIdentifier.PublicServices);
    const viewModel = useMemo(() =>
            new PublicDeliveryPriceViewModel(publicDeliveryService)
        , [publicDeliveryService]);

    const [vatRate, setVatRate] = useState<number>(0);

    useEffect(() => {
        viewModel.getVatRate()
            .then(value => setVatRate(value));
    }, [viewModel, setVatRate])

    return {
        vatRate
    }
}