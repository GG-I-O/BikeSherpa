import {IOCContainer} from "@/bootstrapper/constants/IOCContainer";
import {IDeliveryServices} from "@/deliveries/spi/IDeliveryServices";
import {DeliveryServiceIdentifier} from "@/deliveries/bootstrapper/DeliveryServiceIdentifier";
import {useEffect, useState} from "react";
import {navigate} from "expo-router/build/global-state/routing";
import {observe} from "@legendapp/state";
import DeliveryListViewModel from "@/deliveries/viewModel/DeliveryListViewModel";
import {ICourierService} from "@/spi/CourierSPI";
import {ServicesIdentifiers} from "@/bootstrapper/constants/ServicesIdentifiers";
import {ICustomerService} from "@/spi/CustomerSPI";
import {DeliveryToDisplay} from "@/deliveries/models/DeliveryToDisplay";
import {StepToDisplay} from "@/steps/models/StepToDisplay";
import {IStepServices} from "@/steps/spi/IStepServices";
import {StepServiceIdentifier} from "@/steps/bootstrapper/StepServiceIdentifier";
import defaultCourierList from "@/deliveries/data/defaultCourierDropdown";
import dateFilterEnum from "@/deliveries/data/dateFilterEnum";
import {IDropdownOptionsService} from "@/spi/IDropdownOptionsService";

export default function useDeliveryListViewModel() {
    const deliveryServices = IOCContainer.get<IDeliveryServices>(DeliveryServiceIdentifier.Services);
    const stepServices = IOCContainer.get<IStepServices>(StepServiceIdentifier.Services);
    const courierServices = IOCContainer.get<ICourierService>(ServicesIdentifiers.CourierServices);
    const customerServices = IOCContainer.get<ICustomerService>(ServicesIdentifiers.CustomerServices);
    const dropdownOptionsService = IOCContainer.get<IDropdownOptionsService>(DeliveryServiceIdentifier.DropdownOptionsService);
    const viewModel = new DeliveryListViewModel(deliveryServices, stepServices, courierServices, customerServices, dropdownOptionsService);

    const deliveryStore$ = deliveryServices.getDeliveryList$();
    const customerStore$ = customerServices.getCustomerList$();
    const courierStore$ = courierServices.getCourierList$();

    const [deliveryToDelete, setDeliveryToDelete] = useState<string | null>(null);

    const [deliveries, setDeliveries] = useState<DeliveryToDisplay[]>([]);
    const [steps, setSteps] = useState<StepToDisplay[]>([]);
    const [couriers, setCouriers] = useState<{ label: string, value: string }[]>([]);

    const [datePicker, setDatePicker] = useState<Date|undefined>(new Date());
    const [dateFilter, setDateFilter] = useState<string>(dateFilterEnum.Date);
    const [courierFilter, setCourierFilter] = useState<string[]>([]);

    function displayEditForm(id: string) {
        navigate({
            pathname: '/(tabs)/(deliveries)/edit',
            params: {deliveryId: id}
        });
    }

    function deleteDelivery() {
        if (deliveryToDelete)
            deliveryServices.deleteDelivery(deliveryToDelete);
    }

    useEffect(() => {
        return observe(() => {
            setDeliveries(viewModel.getFilteredDeliveries(dateFilter === dateFilterEnum.Date ? datePicker : undefined));
            setSteps(viewModel.getFilteredStepList(dateFilter === dateFilterEnum.Date ? datePicker : undefined, courierFilter));

            let courierList: { label: string, value: string }[] = [];
            courierList.push(...defaultCourierList);
            Object.values(courierStore$.peek()).forEach(courier =>
                courierList.push({label: courier.code, value: courier.id})
            );
            setCouriers(courierList);
        });

    }, [deliveryStore$, customerStore$, courierStore$, setDeliveries, setSteps, dateFilter, datePicker, courierFilter]);

    return {
        deliveries,
        steps,
        couriers,
        dateFilter,
        setDateFilter,
        datePicker,
        setDatePicker,
        courierFilter,
        setCourierFilter,
        displayEditForm,
        deleteDelivery,
        setDeliveryToDelete,
        assignCourier: viewModel.assignCourier,
        unassignCourier: viewModel.unassignCourier,
    };
}