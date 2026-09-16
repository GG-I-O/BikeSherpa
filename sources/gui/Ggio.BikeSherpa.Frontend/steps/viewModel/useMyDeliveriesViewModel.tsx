import {IOCContainer} from "@/bootstrapper/constants/IOCContainer";
import {IDeliveryServices} from "@/deliveries/spi/IDeliveryServices";
import {DeliveryServiceIdentifier} from "@/deliveries/bootstrapper/DeliveryServiceIdentifier";
import MyDeliveriesViewModel from "@/steps/viewModel/MyDeliveriesViewModel";
import {useEffect, useState} from "react";
import {observe} from "@legendapp/state";
import IDeliveryMapper from "@/deliveries/spi/IDeliveryMapper";
import useDropdown from "@/hooks/useDropdown";
import {StepDisplayForCourier} from "@/steps/models/StepDisplayForCourier";
import {stepDatePickerStore$} from "@/steps/store/StepDatePickerStore";

export default function useMyDeliveriesViewModel() {
    const deliveryServices = IOCContainer.get<IDeliveryServices>(DeliveryServiceIdentifier.Services);
    const deliveryMapper = IOCContainer.get<IDeliveryMapper>(DeliveryServiceIdentifier.Mapper);
    const viewModel = new MyDeliveriesViewModel(deliveryServices, deliveryMapper);

    const deliveryStore$ = deliveryServices.getDeliveryList$();

    const {packingSizes} = useDropdown();

    const [steps, setSteps] = useState<StepDisplayForCourier[]>([]);

    useEffect(() => {
        return stepDatePickerStore$.date.onChange(({value}) => {
            viewModel.loadMyDeliveries(value ?? new Date());
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