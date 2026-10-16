import {useCallback, useEffect} from "react";
import {useFocusEffect} from "expo-router";
import {useValue} from "@legendapp/state/react";
import {IOCContainer} from "@/bootstrapper/constants/IOCContainer";
import {UnassignedBadgeService} from "@/steps/services/UnassignedBadgeService";
import {
    markUnassignedAsSeen,
    unassignedIds$,
    unseenUnassignedCount$
} from "@/steps/store/UnassignedBadgeStore";
import {StepServiceIdentifier} from "@/steps/bootstrapper/StepServiceIdentifier";

export function useUnassignedBadgeSync(enabled: boolean = true): void {
    useEffect(() => {
        if (!enabled) return;
        const service = IOCContainer.get<UnassignedBadgeService>(StepServiceIdentifier.UnassignedBadgeService);
        service.start();
        return () => service.stop();
    }, [enabled]);
}

/** Number of unassigned deliveries the courier hasn't seen yet */
export function useUnseenUnassignedCount(): number {
    return useValue(unseenUnassignedCount$);
}

/**
 * marks everything as seen when the tab gets focus,
 * and again if new deliveries arrive while the courier is looking at the tab.
 */
export function useMarkUnassignedAsSeenOnFocus(): void {
    const ids = useValue(unassignedIds$);

    useFocusEffect(
        useCallback(() => {
            markUnassignedAsSeen();
        }, [ids]) // eslint-disable-line react-hooks/exhaustive-deps
    );
}