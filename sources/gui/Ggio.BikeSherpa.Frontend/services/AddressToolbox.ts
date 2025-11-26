import { Address } from "@/models/Address";
import { Linking, Platform } from "react-native";

export default class AddressToolbox {
    public static openAdressInMaps(address: string) {
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

            Linking.openURL(url ?? '');
        }
        catch (e) {
            console.error('Error opening maps:', e);
        }
    }

    public static async fetchAddress(text: string): Promise<Address[] | null> {
        try {
            const response = await fetch(`https://api-adresse.data.gouv.fr/search/?q${text}&limit=5`);
            if (response.status != 200) {
                throw new Error("Aucune adresse trouvée")
            }
            const data = await response.json();
            return data.features.map((feature: any) => {
                const address: Address = {
                    name: feature.properties.label,
                    streetInfo: feature.properties.name,
                    postcode: feature.properties.postcode,
                    city: feature.properties.city
                }
            })
        } catch (error) {
            console.error(error);
            return null;
        }
    }
}