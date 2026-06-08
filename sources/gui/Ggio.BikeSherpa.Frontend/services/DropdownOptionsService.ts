import {createApiClient} from "@/infra/openAPI/client";
import axios from "axios";
import {DropdownOptions} from "@/models/DropdownOptions";
import {injectable} from "inversify";
import {IDropdownOptionsService} from "@/spi/IDropdownOptionsService";


@injectable()
export default class DropdownOptionsService implements IDropdownOptionsService {
    private apiClient;
    
    private options: Record<string, DropdownOptions[]> = {};

    public constructor() {
        this.apiClient = createApiClient(axios.defaults.baseURL || '', {
            axiosInstance: axios
        });
        this.options["pricingStrategies"] = [];
        this.options["packingsSizes"] = [];
        this.options["urgencies"] = [];
        this.GetOptions().then();
    }
    
    public async GetOptions(): Promise<Record<string, DropdownOptions[]>> {
        if(this.options["pricingStrategies"].length === 0) {
            this.options["pricingStrategies"] = await this.getAllPricingStrategies();
        }

        if(this.options["packingsSizes"].length === 0) {
            this.options["packingsSizes"] = await this.getAllPackingSizes();
        }

        if(this.options["urgencies"].length === 0) {
            this.options["urgencies"] = await this.getAllUrgencies();
        }
        
        return this.options;
    }
    
    public GetPackingLabel = (packing: string): string => {
        if (!this.options)
            return packing;
        return this.options["packingsSizes"].find((option) => option.value === packing)?.label ?? 'Unknown';
    }

    private async getAllPricingStrategies(): Promise<{ label: string, value: string }[]> {
        const data = await this.apiClient.GetAllPricingStrategiesEndpoint();

        return data.map(
            strategy => {
                return {
                    label: strategy.label,
                    value: strategy.value.toString()
                }
            }
        );
    }

    private async getAllPackingSizes(): Promise<{ label: string, value: string }[]> {
        return await this.apiClient.GetAllPackingSizesEndpoint();
    }

    private async getAllUrgencies(): Promise<{ label: string, value: string }[]> {
        return await this.apiClient.GetAllUrgenciesEndpoint();
    }
}