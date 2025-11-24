import { Card, Divider, Text, useTheme } from "react-native-paper";
import { Delivery } from "../models/Delivery";
import AppStyle from "@/constants/AppStyle";

type Props = {
    delivery: Delivery,
    isSelected?: boolean,
    onPress?: (delivery: Delivery) => void
}

export default function DeliveryCard({ delivery, isSelected = false, onPress }: Props) {
    const theme = useTheme();

    return (
        <Card
            style={{ backgroundColor: isSelected ? theme.colors.primary : theme.colors.background }}
            onPress={() => {
                if (onPress) onPress(delivery);
            }}
        >
            <Card.Content style={{ alignItems: 'center', gap: 4 }} >
                <Text style={AppStyle.textStyle.h2}>{delivery.code}</Text>
                <Divider />
                <Text style={{ alignSelf: 'flex-start' }}>{delivery.steps ? delivery.steps[0].getContractDate() : ''}</Text>
                <Text style={{ alignSelf: 'flex-start' }}>{delivery.steps ? delivery.steps[0].getContractTime() : ''}</Text>
                <Text style={{ alignSelf: 'flex-end' }}>Éstape : {delivery.steps ? delivery.steps.length : ''}</Text>
            </Card.Content>
        </Card>
    );
}