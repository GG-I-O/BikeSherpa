import {IconButton, useTheme} from "react-native-paper";
import {DeliveryStatusEnum} from "@/deliveries/data/deliveryStatusEnum";
import useDeliveryStatusButtonViewModel from "@/deliveries/viewModel/useDeliveryStatusButtonViewModel";

type Props = {
    deliveryId: string,
    status: DeliveryStatusEnum,
    isActive?: boolean
}

export default function DeliveryStatusButton(props: Props) {
    const theme = useTheme();
    const viewModel = useDeliveryStatusButtonViewModel();
    
    let iconButton: {icon: string, color: string, onPress: (deliveryId: string) => void};
    switch (props.status) {
        case DeliveryStatusEnum.New:
            iconButton = {icon: "new-box", color: theme.colors.error, onPress: viewModel.changeStatusToPending};
            break;
            
        case DeliveryStatusEnum.Pending:
            iconButton = {icon: "clock", color: theme.colors.primary, onPress: viewModel.changeStatusToNew};
            break;

        case DeliveryStatusEnum.Started:
            iconButton = {icon: "circle-slice-1", color: theme.colors.secondary, onPress: deliveryId => {}};
            break;

        case DeliveryStatusEnum.Completed:
            iconButton = {icon: "check-circle", color: "#60cc70", onPress: deliveryId => {}};
            break;

        case DeliveryStatusEnum.Cancelled:
            iconButton = {icon: "alert", color: theme.colors.error, onPress: deliveryId => {}};
            break;
    }

    return <IconButton
        icon={iconButton.icon}
        size={28}
        iconColor={iconButton.color}
        contentStyle={{alignItems: 'flex-start'}}
        onPress={() => iconButton.onPress(props.deliveryId)}
        disabled={props.isActive ?? false}
    />
}