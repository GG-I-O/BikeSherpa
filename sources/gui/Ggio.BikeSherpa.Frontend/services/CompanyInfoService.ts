import {inject, injectable} from "inversify";
import {ICompanyInfoService} from "@/spi/ICompanyInfoService";
import {ILogger} from "@/spi/LogsSPI";
import {ServicesIdentifiers} from "@/bootstrapper/constants/ServicesIdentifiers";
import {createApiClient} from "@/infra/openAPI/client";
import axios from "axios";

@injectable()
export default class CompanyInfoService implements ICompanyInfoService {
    private readonly logger: ILogger;
    private readonly apiClient;

    constructor(
        @inject(ServicesIdentifiers.Logger) logger: ILogger
    ) {
        this.logger = logger.extend("CompanyInfoService");

        this.apiClient = createApiClient(axios.defaults.baseURL || '', {
            axiosInstance: axios
        });
    }

    public async getCompanyLabel(): Promise<string> {
        try {
            return await this.apiClient.GetCompanyLabelEndpoint();
        } catch (e) {
            this.logger.error("Could not fetch company label :", e);
            return "Bike Sherpa";
        }
    }

    public async getCompanyPhone(): Promise<string | undefined> {
        try {
            return await this.apiClient.GetCompanyPhoneEndpoint();
        } catch (e) {
            this.logger.error("Could not fetch company phone :", e);
            return undefined;
        }
    }
}