import {IOCContainer} from "@/bootstrapper/constants/IOCContainer";
import {DeliveryServiceIdentifier} from "@/deliveries/bootstrapper/DeliveryServiceIdentifier";
import {IDeliveryServices} from "@/deliveries/spi/IDeliveryServices";
import DeliveryStatusButtonViewModel from "@/deliveries/viewModel/DeliveryStatusButtonViewModel";

export default function useDeliveryStatusButtonViewModel() {
    const deliveryService = IOCContainer.get<IDeliveryServices>(DeliveryServiceIdentifier.Services);
    const viewModel = new DeliveryStatusButtonViewModel(deliveryService);
    
    return {
        changeStatusToNew: (deliveryId: string) => viewModel.changeStatusToNew(deliveryId),
        changeStatusToPending: (deliveryId: string) => viewModel.changeStatusToPending(deliveryId),
    }
}