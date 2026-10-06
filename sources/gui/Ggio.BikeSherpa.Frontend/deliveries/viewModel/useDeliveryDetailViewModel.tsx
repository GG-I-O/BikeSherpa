import {IOCContainer} from "@/bootstrapper/constants/IOCContainer";
import {IDeliveryServices} from "@/deliveries/spi/IDeliveryServices";
import {DeliveryServiceIdentifier} from "@/deliveries/bootstrapper/DeliveryServiceIdentifier";
import DeliveryDetailViewModel from "@/deliveries/viewModel/DeliveryDetailViewModel";
import IDeliveryMapper from "@/deliveries/spi/IDeliveryMapper";
import {useState} from "react";
import {IProofOfDeliveryService} from "@/deliveries/spi/IProofOfDeliveryService";

export default function useDeliveryDetailViewModel() {
    const deliveryServices = IOCContainer.get<IDeliveryServices>(DeliveryServiceIdentifier.Services);
    const deliveryMapper = IOCContainer.get<IDeliveryMapper>(DeliveryServiceIdentifier.Mapper);
    const proofOfDeliveryService = IOCContainer.get<IProofOfDeliveryService>(DeliveryServiceIdentifier.ProofOfDeliveryService);
    const viewModel = new DeliveryDetailViewModel(deliveryServices, deliveryMapper, proofOfDeliveryService);
    
    const [proofOfDeliveryLink, setProofOfDeliveryLink] = useState<string | null>(null);
    
    return {
        getDelivery: (id: string) => viewModel.getDelivery(id),
        proofOfDeliveryLink,
        exportProofOfDelivery: (deliveryId: string) => viewModel.exportProofOfDelivery(deliveryId).then((result) => setProofOfDeliveryLink(result))
    };
}