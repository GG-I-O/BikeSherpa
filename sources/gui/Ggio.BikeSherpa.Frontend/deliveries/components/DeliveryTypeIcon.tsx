import { StepType } from "@/steps/models/StepType";
import { Icon, useTheme } from "react-native-paper";

type Props = {
    type: StepType;
}

export default function DeliveryTypeIcon({ type }: Props) {
    const theme = useTheme();

    return <Icon source={type == StepType.PickUp ? "archive-arrow-up-outline" : "arrow-down-bold"} size={28} color={theme.colors.onBackground} />
}