import ThemedCardList from "@/components/themed/ThemedCardList";
import { Delivery } from "../models/Delivery";
import DeliveryCard from "./DeliveryCard";

type Props = {
    deliveries: Array<Delivery>,
    isDeliverySelected?: (delivery: Delivery) => boolean,
    onDeliveryPress?: (delivery: Delivery) => void
}

export default function DeliveryCardList({ deliveries, isDeliverySelected, onDeliveryPress }: Props) {
    return (
        <ThemedCardList
            data={deliveries}
            card={({ item }) => {
                const delivery = item as Delivery;
                return (
                    <DeliveryCard
                        delivery={delivery}
                        isSelected={isDeliverySelected ? isDeliverySelected(delivery) : false}
                        onPress={onDeliveryPress}
                    />
                );
            }}
        />
    );
}