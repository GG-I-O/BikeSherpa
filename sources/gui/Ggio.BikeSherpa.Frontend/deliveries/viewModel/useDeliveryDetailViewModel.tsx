import {IOCContainer} from "@/bootstrapper/constants/IOCContainer";
import {IDeliveryServices} from "@/deliveries/spi/IDeliveryServices";
import {DeliveryServiceIdentifier} from "@/deliveries/bootstrapper/DeliveryServiceIdentifier";
import DeliveryDetailViewModel from "@/deliveries/viewModel/DeliveryDetailViewModel";
import IDeliveryMapper from "@/deliveries/spi/IDeliveryMapper";

export default function useDeliveryDetailViewModel() {
    const deliveryServices = IOCContainer.get<IDeliveryServices>(DeliveryServiceIdentifier.Services);
    const deliveryMapper = IOCContainer.get<IDeliveryMapper>(DeliveryServiceIdentifier.Mapper);
    const viewModel = new DeliveryDetailViewModel(deliveryServices, deliveryMapper);
    
    return {
        getDelivery: (id: string) => viewModel.getDelivery(id),  
    };
}