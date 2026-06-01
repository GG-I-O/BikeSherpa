import { observable } from "@legendapp/state";
import PublicDeliveryCustomer from "@/deliveries/models/PublicDeliveryCustomer";
import {DeliveryToDisplay} from "@/deliveries/models/DeliveryToDisplay";

type PublicDeliveryState = {
    customer: PublicDeliveryCustomer | null;
    isAnonymous: boolean;
    delivery: DeliveryToDisplay | null;
};

export const publicDeliveryStore$ = observable<PublicDeliveryState>({
    customer: null,
    isAnonymous: false,
    delivery: null,
});