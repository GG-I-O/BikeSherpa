import { observable } from "@legendapp/state";
import PublicDeliveryCustomer from "@/deliveries/models/PublicDeliveryCustomer";
import Delivery from "@/deliveries/models/Delivery";
import PublicDeliveryEstimatedValue from "@/deliveries/models/PublicDeliveryEstimatedValue";

type PublicDeliveryState = {
    customer: PublicDeliveryCustomer | null;
    isAnonymous: boolean;
    delivery: Delivery | null;
    estimatedValue: PublicDeliveryEstimatedValue | null;
    isLoadingModalVisible: boolean;
    isErrorModalVisible: boolean;
};

export const publicDeliveryStore$ = observable<PublicDeliveryState>({
    customer: null,
    isAnonymous: false,
    delivery: null,
    estimatedValue: null,
    isLoadingModalVisible: false,
    isErrorModalVisible: false,
});