import {ReportServiceIdentifier} from "@/reports/bootstrapper/ReportServiceIdentifier";
import ReportServices from "@/reports/services/ReportServices";
import {Container} from "inversify";
import {IReportServices} from "@/reports/spi/IReportServices";
import IReportMapper from "@/reports/spi/IReportMapper";
import ReportMapper from "@/reports/services/ReportMapper";

export default class ReportBootstrapper {
    public static init(IOCContainer: Container) {
        IOCContainer.bind<IReportServices>(ReportServiceIdentifier.Services).to(ReportServices).inSingletonScope();
        
        IOCContainer.bind<IReportMapper>(ReportServiceIdentifier.Mapper).to(ReportMapper).inSingletonScope();
    }
}