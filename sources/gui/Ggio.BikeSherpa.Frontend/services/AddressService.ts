import { ServicesIdentifiers } from "@/bootstrapper/constants/ServicesIdentifiers";
import { Address } from "@/models/Address";
import { IAddressService } from "@/spi/AddressSPI";
import { ILogger } from "@/spi/LogsSPI";
import { inject, injectable } from "inversify";
import { Linking, Platform } from "react-native";
import {createApiClient} from "@/infra/openAPI/client";
import axios from "axios";

@injectable()
export default class AddressService implements IAddressService {
    private readonly logger: ILogger;
    private readonly apiClient;

    constructor(
        @inject(ServicesIdentifiers.Logger) logger: ILogger
    ) {
        this.logger = logger;
        
        this.apiClient = createApiClient(axios.defaults.baseURL || '', {
            axiosInstance: axios
        });
    }

    public openAddressInMaps(address: string) {
        try {
            const encodedAddress = encodeURIComponent(address);

            let url: string;

            if (Platform.OS === 'web') {
                url = `https://www.google.com/maps/search/?api=1&query=${encodedAddress}`;
            }
            else {
                url = Platform.select({
                    ios: `maps:0,0?q=${encodedAddress}`,
                    android: `geo:0,0?q=${encodedAddress}`,
                }) ?? '';
            }

            Linking.openURL(url ?? '').then();
        }
        catch (e) {
            this.logger.error('Error opening maps:', e);
        }
    }

    public async fetchAddress(text: string): Promise<Address[] | null> {
        try {
            const response = await this.apiClient.GetAddressSuggestionEndpoint({
                params: {query: text}
            });
            
            return response.map(suggestedAddress => {
                const address: Address = {
                    name: suggestedAddress.name,
                    phone: "",
                    fullAddress: "",
                    streetInfo: suggestedAddress.streetInfo,
                    complement: null,
                    postcode: suggestedAddress.postcode,
                    city: suggestedAddress.city,
                    coordinates: {
                        longitude: Number(suggestedAddress.coordinates.longitude),
                        latitude: Number(suggestedAddress.coordinates.latitude)
                    }
                }
                address.fullAddress = `${address.streetInfo} ${address.postcode} ${address.city}`;
                return address;
            })
        } catch (error) {
            this.logger.error(error);
            return null;
        }
    }
}