import {IOCContainer} from "@/bootstrapper/constants/IOCContainer";
import {DeliveryServiceIdentifier} from "@/deliveries/bootstrapper/DeliveryServiceIdentifier";
import DeliveryDetailViewModel from "@/deliveries/viewModel/DeliveryDetailViewModel";
import {useState} from "react";
import {IProofOfDeliveryService} from "@/deliveries/spi/IProofOfDeliveryService";
import {IReportServices} from "@/reports/spi/IReportServices";
import {ReportServiceIdentifier} from "@/reports/bootstrapper/ReportServiceIdentifier";
import {Report} from "@/reports/models/Report";

export default function useDeliveryDetailViewModel() {
    const reportService = IOCContainer.get<IReportServices>(ReportServiceIdentifier.Services);
    const proofOfDeliveryService = IOCContainer.get<IProofOfDeliveryService>(DeliveryServiceIdentifier.ProofOfDeliveryService);
    const viewModel = new DeliveryDetailViewModel(reportService, proofOfDeliveryService);
    
    const [deliveryReport, setDeliveryReport] = useState<Report | null>(null);
    const [proofOfDeliveryLink, setProofOfDeliveryLink] = useState<string | null>(null);
    
    return {
        loadDeliveryReport: (id: string) => viewModel.getDeliveryReport(id).then((result) => setDeliveryReport(result)),
        deliveryReport,
        proofOfDeliveryLink,
        exportProofOfDelivery: (deliveryId: string) => viewModel.exportProofOfDelivery(deliveryId).then((result) => setProofOfDeliveryLink(result))
    };
}