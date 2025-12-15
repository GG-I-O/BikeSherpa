import { Step } from "@/steps/models/Step";
import { useCallback, useState } from "react";
import { Delivery } from "../models/Delivery";

export function useDeliverySelection() {
    const [selectedSteps, setSelectedSteps] = useState<Step[]>([]);
    const [selectedDeliveries, setSelectedDeliveries] = useState<Delivery[]>([]);

    const isStepSelected = useCallback((step: Step) => {
        return selectedSteps.some(s => s.id === step.id);
    }, [selectedSteps]);

    const isDeliverySelected = useCallback((delivery: Delivery) => {
        return selectedDeliveries.some(d => d.id === delivery.id)
    }, [selectedDeliveries]);

    const toggleDeliverySelection = useCallback((delivery: Delivery) => {
        if (!delivery.steps) return;

        const isCurrentlySelected = selectedDeliveries.some((d) => d.id === delivery.id);

        if (isCurrentlySelected) {
            setSelectedSteps(selectedSteps.filter((step: Step) => !delivery.steps?.some((s: Step) => s.id === step.id)));
            setSelectedDeliveries(selectedDeliveries.filter((d: Delivery) => d.id !== delivery.id));
        }
        else {
            const stepsToAdd = delivery.steps?.filter((step: Step) =>
                !selectedSteps.some(s => s.id === step.id)
            );
            setSelectedSteps(current => current.concat(stepsToAdd ?? []));
            setSelectedDeliveries(current => current.concat([delivery]));
        }
    }, [selectedSteps, selectedDeliveries]);

    const toggleStepSelection = useCallback((step: Step, delivery: Delivery) => {
        const isSelected = selectedSteps.some((s: Step) => s.id === step.id);
        if (isSelected) {
            setSelectedSteps(selectedSteps.filter((s: Step) => s.id !== step.id));
            setSelectedDeliveries(selectedDeliveries.filter((d: Delivery) => d.id !== delivery.id));
        }
        else {
            const tempSelectedSteps = selectedSteps.concat([step]);
            setSelectedSteps(tempSelectedSteps);
            const doWeAddDelivery = delivery.steps?.every((s: Step) => tempSelectedSteps.some((ss: Step) => ss.id === s.id));
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