import {useEffect} from "react";
import {useValue} from "@legendapp/state/react";
import {IOCContainer} from "@/bootstrapper/constants/IOCContainer";
import {IUnassignedBadgeService} from "@/steps/spi/IUnassignedBadgeService";
import {unassignedStepCount$} from "@/steps/store/UnassignedBadgeStore";
import {StepServiceIdentifier} from "@/steps/bootstrapper/StepServiceIdentifier";

export function useUnassignedBadgeSync(enabled: boolean = true): void {
    useEffect(() => {
        if (!enabled) return;
        const service = IOCContainer.get<IUnassignedBadgeService>(StepServiceIdentifier.UnassignedBadgeService);
        service.start();
        return () => service.stop();
    }, [enabled]);
}

export function useUnassignedStepCount(): number {
    return useValue(unassignedStepCount$);
}