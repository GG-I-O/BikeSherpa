import { observable } from "@legendapp/state";
import { ObservablePersistAsyncStorage } from "@legendapp/state/persist-plugins/async-storage";

type AuthFallbackStore = {
    dpopDisabled: boolean;
};

export const authFallbackStore$ = observable<AuthFallbackStore>({
    dpopDisabled: false,
});