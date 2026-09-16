import {IOCContainer} from "@/bootstrapper/constants/IOCContainer";
import {IDeliveryServices} from "@/deliveries/spi/IDeliveryServices";
import {DeliveryServiceIdentifier} from "@/deliveries/bootstrapper/DeliveryServiceIdentifier";
import {useEffect, useState} from "react";
import {observe} from "@legendapp/state";
import IDeliveryMapper from "@/deliveries/spi/IDeliveryMapper";
import useDropdown from "@/hooks/useDropdown";
import UnassignedDeliveriesViewModel from "@/steps/viewModel/UnassignedDeliveriesViewModel";
import {StepDisplayForCourier} from "@/steps/models/StepDisplayForCourier";
import {stepDatePickerStore$} from "@/steps/store/StepDatePickerStore";

export default function useUnassignedDeliveriesViewModel() {
    const deliveryServices = IOCContainer.get<IDeliveryServices>(DeliveryServiceIdentifier.Services);
    const deliveryMapper = IOCContainer.get<IDeliveryMapper>(DeliveryServiceIdentifier.Mapper);
    const viewModel = new UnassignedDeliveriesViewModel(deliveryServices, deliveryMapper);

    const deliveryStore$ = deliveryServices.getDeliveryList$();

    const {packingSizes} = useDropdown();

    const [steps, setSteps] = useState<StepDisplayForCourier[]>([]);

    useEffect(() => {
        return stepDatePickerStore$.date.onChange(({value}) => {
            viewModel.loadDeliveries(value ?? new Date());
        }, {initial: true});
    }, []); // eslint-disable-line react-hooks/exhaustive-deps

    useEffect(() => {
        return observe(() => {
            setSteps(viewModel.getSteps());
        });
    }, [deliveryStore$, packingSizes, setSteps]); // eslint-disable-line react-hooks/exhaustive-deps

    return {
        steps
    }
}