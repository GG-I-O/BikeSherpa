import {IOCContainer} from "@/bootstrapper/constants/IOCContainer";
import IPublicDeliveryService from "@/deliveries/spi/IPublicDeliveryService";
import {DeliveryServiceIdentifier} from "@/deliveries/bootstrapper/DeliveryServiceIdentifier";
import {useEffect, useMemo, useRef, useState} from "react";
import PublicDeliveryDetailsFormViewModel from "@/deliveries/viewModel/PublicDeliveryDetailsFormViewModel";
import DateToolbox from "@/services/DateToolbox";
import {DropdownOptions} from "@/models/DropdownOptions";
import usePublicDeliveryModal from "@/deliveries/hooks/usePublicDeliveryModal";

type UrgencyOption = { value: string; label: string; lastHourToOrder: number };

export default function usePublicDeliveryDetailsFormViewModel(
    startDate: Date,
    setStartDate: (startDate: string) => void,
    urgency: string,
    setUrgency: (urgency: string) => void
) {
    const publicDeliveryService = IOCContainer.get<IPublicDeliveryService>(
        DeliveryServiceIdentifier.PublicServices
    );

    // Stable service instance
    const viewModel = useMemo(
        () => new PublicDeliveryDetailsFormViewModel(publicDeliveryService),
        []
    );

    const {setIsLoadingModalVisible} = usePublicDeliveryModal();

    // Stable "now"
    const nowRef = useRef(new Date());
    const now = nowRef.current;

    // Static async data fetched once
    const [lastHourToOrder, setLastHourToOrder] = useState<number>(15);
    const [workHours, setWorkHours] = useState<{ start: Date; end: Date } | null>(null);
    const [allUrgencies, setAllUrgencies] = useState<UrgencyOption[]>([]);

    useEffect(() => {
        const loadData = async () => {
            setIsLoadingModalVisible(true);

            const resultLastHourToOrder = await viewModel.getLastHourToOrder()
            setLastHourToOrder(resultLastHourToOrder);

            const resultWorkHours = await viewModel.getWorkHours();
            setWorkHours({
                start: new Date(resultWorkHours.startDate),
                end: new Date(resultWorkHours.endDate)
            })

            const resultUrgenciesLastHourToOrder = await viewModel.getUrgenciesLastHourToOrder();
            setAllUrgencies(resultUrgenciesLastHourToOrder);
        }

        loadData().finally(() => setIsLoadingModalVisible(false))
    }, [viewModel, setIsLoadingModalVisible]);

    // Helper field
    const isFieldToday = DateToolbox.dateFilterFunction(now, startDate);

    // Available hours based on date and work hours
    const hoursOptions = useMemo<DropdownOptions[]>(() => {
        const startHours = isFieldToday ? now.getHours() + 1 : workHours?.start.getHours() ?? 8;
        const endHours = workHours?.end.getHours() ?? 17;
        const options: DropdownOptions[] = [];
        for (let i = startHours; i < endHours; i++) {
            options.push({label: i.toString(), value: i.toString()});
        }
        return options;
    }, [isFieldToday, now, workHours]);

    // Minutes options
    const minutesOptions = useMemo<DropdownOptions[]>(() => {
        const options: DropdownOptions[] = [];
        for (let i = 0; i < 60; i += 15) {
            options.push({label: i.toString(), value: i.toString()});
        }
        return options;
    }, []);

    // Filtered urgencies based on current startDate
    const possibleUrgencies = useMemo<UrgencyOption[]>(() => {
        if (!isFieldToday) return allUrgencies;
        return allUrgencies.filter((u) => u.lastHourToOrder > startDate.getHours());
    }, [allUrgencies, isFieldToday, startDate]);

    // Correct form values when they fall outside possible ranges
    useEffect(() => {
        if (!isFieldToday) return;

        const corrected = new Date(startDate);

        // If hour is later than last possible hour, set to next day at first hour
        const canDoSameDay = (startDate.getHours() + 1) <= (workHours?.end.getHours() ?? 17);
        if (!canDoSameDay) {
            corrected.setDate(corrected.getDate() + 1);
            corrected.setHours(workHours?.start.getHours() ?? 8);
            corrected.setMinutes(0);
            setStartDate(corrected.toISOString());
        }
        
        if (possibleUrgencies.length === 0 || hoursOptions.length === 0) return;

        const urgencyStillValid = possibleUrgencies.some((u) => u.value === urgency);
        if (!urgencyStillValid) {
            setUrgency(possibleUrgencies[0].value);
        }
        
        // If hour is before possible hours, set to first possible hour
        const hourStillValid = hoursOptions.some((h) => h.value === startDate.getHours().toString());
        if (!hourStillValid) {
            corrected.setHours(parseInt(hoursOptions[0].value));
            corrected.setMinutes(0);
            setStartDate(corrected.toISOString());
        }        
    }, [possibleUrgencies, hoursOptions, workHours]); // Do not add dependencies that the linter asks, it's intentional

    return {
        urgencies: possibleUrgencies,
        lastHourToOrder,
        workHours,
        hoursOptions,
        minutesOptions,
    };
}