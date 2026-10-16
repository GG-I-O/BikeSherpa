import {observable} from "@legendapp/state";
import {synced} from "@legendapp/state/sync";
import {ObservablePersistMMKV} from "@legendapp/state/persist-plugins/mmkv";

/** Ids of the deliveries currently unassigned. Not persisted: always refetched. */
export const unassignedIds$ = observable<string[]>([]);

/** Ids the courier already saw by opening the "Non assignées" tab. Persisted. */
export const seenUnassignedIds$ = observable(synced({
    initial: [] as string[],
    persist: {
        plugin: ObservablePersistMMKV,
        name: 'unassignedSeenIds',
    },
}));

/** Computed: number of unassigned deliveries not seen yet. Drives the tab badge. */
export const unseenUnassignedCount$ = observable((): number => {
    const seen = new Set(seenUnassignedIds$.get());
    return unassignedIds$.get().filter((id) => !seen.has(id)).length;
});

export function markUnassignedAsSeen(): void {
    seenUnassignedIds$.set([...unassignedIds$.peek()]);
}