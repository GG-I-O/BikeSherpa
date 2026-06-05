import {IOCContainer} from "@/bootstrapper/constants/IOCContainer";
import IPublicDeliveryService from "@/deliveries/spi/IPublicDeliveryService";
import {DeliveryServiceIdentifier} from "@/deliveries/bootstrapper/DeliveryServiceIdentifier";
import {useEffect, useMemo, useRef, useState} from "react";
import PublicDeliveryDetailsFormViewModel from "@/deliveries/viewModel/PublicDeliveryDetailsFormViewModel";
import DateToolbox from "@/services/DateToolbox";
import {DropdownOptions} from "@/models/DropdownOptions";

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

    // Stable "now"
    const nowRef = useRef(new Date());
    const now = nowRef.current;

    // Static async data fetched once
    const [lastHourToOrder, setLastHourToOrder] = useState<number>(15);
    const [workHours, setWorkHours] = useState<{ start: Date; end: Date } | null>(null);
    const [allUrgencies, setAllUrgencies] = useState<UrgencyOption[]>([]);

    useEffect(() => {
        viewModel.getLastHourToOrder()
            .then(setLastHourToOrder);
        viewModel.getWorkHours()
            .then((wh) =>
                setWorkHours({
                    start: new Date(wh.startDate),
                    end: new Date(wh.endDate)
                })
            );
        viewModel.getUrgenciesLastHourToOrder()
            .then(setAllUrgencies);
    }, [viewModel]);

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
        if (possibleUrgencies.length === 0 || hoursOptions.length === 0) return;

        const urgencyStillValid = possibleUrgencies.some((u) => u.value === urgency);
        if (!urgencyStillValid) {
            setUrgency(possibleUrgencies[0].value);
        }

        const hourStillValid = hoursOptions.some(
            (h) => parseInt(h.value) === startDate.getHours()
        );
        if (!hourStillValid) {
            const corrected = new Date(startDate);
            corrected.setHours(parseInt(hoursOptions[0].value));
            corrected.setMinutes(0);
            setStartDate(corrected.toISOString());
        }
    }, [possibleUrgencies, hoursOptions]); // Do not add dependencies that the linter asks, it's intentional

    return {
        urgencies: possibleUrgencies,
        lastHourToOrder,
        workHours,
        hoursOptions,
        minutesOptions,
    };
}