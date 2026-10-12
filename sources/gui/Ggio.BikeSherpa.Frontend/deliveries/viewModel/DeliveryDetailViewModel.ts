import {DeliveryServiceIdentifier} from "@/deliveries/bootstrapper/DeliveryServiceIdentifier";
import {inject} from "inversify";
import {Report} from "@/reports/models/Report";
import {IProofOfDeliveryService} from "@/deliveries/spi/IProofOfDeliveryService";
import {IReportServices} from "@/reports/spi/IReportServices";
import {ReportServiceIdentifier} from "@/reports/bootstrapper/ReportServiceIdentifier";

export default class DeliveryDetailViewModel {
    private readonly reportService: IReportServices;
    private readonly proofOfDeliveryService: IProofOfDeliveryService;

    constructor(
       @inject(ReportServiceIdentifier.Services) reportService: IReportServices,
        @inject(DeliveryServiceIdentifier.ProofOfDeliveryService) proofOfDeliveryService: IProofOfDeliveryService
    ) {
        this.reportService = reportService;
        this.proofOfDeliveryService = proofOfDeliveryService;
    }

    public getDeliveryReport = async (id: string): Promise<Report> => {
        return await this.reportService.getDeliveryReport(id);
    }
    
    public exportProofOfDelivery = async (deliveryId: string): Promise<string> => {
        return await this.proofOfDeliveryService.exportProofOfDelivery(deliveryId);
    }
}