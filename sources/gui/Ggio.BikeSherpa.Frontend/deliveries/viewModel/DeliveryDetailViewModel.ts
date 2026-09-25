import {IDeliveryServices} from "@/deliveries/spi/IDeliveryServices";
import {DeliveryServiceIdentifier} from "@/deliveries/bootstrapper/DeliveryServiceIdentifier";
import {inject} from "inversify";
import {DeliveryToDisplay} from "@/deliveries/models/DeliveryToDisplay";
import Delivery from "@/deliveries/models/Delivery";
import IDeliveryMapper from "@/deliveries/spi/IDeliveryMapper";
import {IProofOfDeliveryService} from "@/deliveries/spi/IProofOfDeliveryService";

export default class DeliveryDetailViewModel {
    private readonly deliveryServices: IDeliveryServices;
    private readonly deliveryMapper: IDeliveryMapper;
    private readonly proofOfDeliveryService: IProofOfDeliveryService;

    constructor(
        @inject(DeliveryServiceIdentifier.Services) deliveryServices: IDeliveryServices,
        @inject(DeliveryServiceIdentifier.Mapper) deliveryMapper: IDeliveryMapper,
        @inject(DeliveryServiceIdentifier.ProofOfDeliveryService) proofOfDeliveryService: IProofOfDeliveryService
    ) {
        this.deliveryServices = deliveryServices;
        this.deliveryMapper = deliveryMapper;
        this.proofOfDeliveryService = proofOfDeliveryService;
    }

    public getDelivery = (id: string): DeliveryToDisplay => {
        const delivery: Delivery = this.deliveryServices.getDelivery$(id).get();

        return this.deliveryMapper.DeliveryToDeliveryToDisplay(delivery);
    }
    
    public exportProofOfDelivery = async (deliveryId: string): Promise<string> => {
        return await this.proofOfDeliveryService.exportProofOfDelivery(deliveryId);
    }
}