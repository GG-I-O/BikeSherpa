import { useCallback, useState } from "react";
import {DeliveryToDisplay} from "@/deliveries/models/DeliveryToDisplay";
import {StepToDisplay} from "@/steps/models/StepToDisplay";

export function useDeliverySelection() {
    const [selectedSteps, setSelectedSteps] = useState<StepToDisplay[]>([]);
    const [selectedDeliveries, setSelectedDeliveries] = useState<DeliveryToDisplay[]>([]);

    const isStepSelected = useCallback((step: StepToDisplay) => {
        return selectedSteps.some(s => s.id === step.id);
    }, [selectedSteps]);

    const isDeliverySelected = useCallback((delivery: DeliveryToDisplay) => {
        return selectedDeliveries.some(d => d.id === delivery.id)
    }, [selectedDeliveries]);

    const toggleDeliverySelection = useCallback((delivery: DeliveryToDisplay) => {
        if (!delivery.steps) return;

        const isCurrentlySelected = selectedDeliveries.some((d) => d.id === delivery.id);

        if (isCurrentlySelected) {
            setSelectedSteps(selectedSteps.filter((step: StepToDisplay) => !delivery.steps?.some((s: StepToDisplay) => s.id === step.id)));
            setSelectedDeliveries(selectedDeliveries.filter((d: DeliveryToDisplay) => d.id !== delivery.id));
        }
        else {
            const stepsToAdd = delivery.steps?.filter((step: StepToDisplay) =>
                !selectedSteps.some(s => s.id === step.id)
            );
            setSelectedSteps(current => current.concat(stepsToAdd ?? []));
            setSelectedDeliveries(current => current.concat([delivery]));
        }
    }, [selectedSteps, selectedDeliveries]);

    const toggleStepSelection = useCallback((step: StepToDisplay, delivery: DeliveryToDisplay) => {
        const isSelected = selectedSteps.some((s: StepToDisplay) => s.id === step.id);
        if (isSelected) {
            setSelectedSteps(selectedSteps.filter((s: StepToDisplay) => s.id !== step.id));
            setSelectedDeliveries(selectedDeliveries.filter((d: DeliveryToDisplay) => d.id !== delivery.id));
        }
        else {
            const tempSelectedSteps = selectedSteps.concat([step]);
            setSelectedSteps(tempSelectedSteps);
            const doWeAddDelivery = delivery.steps?.every((s: StepToDisplay) => tempSelectedSteps.some((ss: StepToDisplay) => ss.id === s.id));
            if (doWeAddDelivery)
                setSelectedDeliveries(selectedDeliveries.concat([delivery]));
        }
    }, [selectedSteps, selectedDeliveries]);

    const clearSelection = useCallback(() => {
        setSelectedSteps([]);
        setSelectedDeliveries([]);
    }, [setSelectedSteps, setSelectedDeliveries]);

    return {
        selectedSteps,
        isStepSelected,
        isDeliverySelected,
        toggleStepSelection,
        toggleDeliverySelection,
        clearSelection
    }
}